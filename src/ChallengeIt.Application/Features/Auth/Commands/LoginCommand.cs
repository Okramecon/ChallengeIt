using ChallengeIt.Application.Features.Auth.Results;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Domain.Entities;
using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Auth.Commands;

public record LoginCommand(string Login, string Password) : IRequest<ErrorOr<LoginResult>>;

public class LoginCommandHandler(
    IPasswordHasher passwordHasher,
    IUsersRepository usersRepository,
    ITokenProvider tokenProvider)
    : IRequestHandler<LoginCommand, ErrorOr<LoginResult>>
{
    public async Task<ErrorOr<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Login) && string.IsNullOrEmpty(request.Password))
            return Errors.LoginDataIsEmpty;

        User? user = await usersRepository.GetByUserNameAsync(request.Login, cancellationToken) 
            ?? await usersRepository.GetByEmailAsync(request.Login, cancellationToken);

        if (user is null)
            return Errors.UserNotFound;

        if (user.IsExternal)
            return Errors.ExternalUserLogin;

        if (!passwordHasher.Verify(user.PasswordHash!, request.Password))
            return Errors.InvalidCredentials;

        var (refreshTokenValue, refreshExpiresAt) = tokenProvider.GenerateRefreshToken();

        var refreshToken = await usersRepository.GetRefreshTokenAsync(user.Id, cancellationToken) ?? new RefreshToken()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = ""
        };
        refreshToken.ExpiresAt = refreshExpiresAt;
        refreshToken.Token = refreshTokenValue;

        await usersRepository.UpdateRefreshTokenAsync(refreshToken, cancellationToken);

        var accessToken = tokenProvider.GenerateJwtToken(user.Id, user.Username, user.Email, refreshToken.Id);
        return new LoginResult(accessToken, refreshToken.Token);
    }

    public static class Errors
    {
        public const string Code = "Auth.Login";

        public static Error UserNotFound => Error.NotFound(
            code: Code,
            description: "User was not found");

        public static Error LoginDataIsEmpty => Error.Validation(
            code: Code,
            description: "Login model has to provide username or email");

        public static Error ExternalUserLogin => Error.Validation(
            code: Code,
            description: "User is external and cannot login with password. Try login with SSO.");

        public static Error NotFound => Error.NotFound(
            code: Code,
            description: "Breakfast not found");

        public static Error InvalidCredentials => Error.Unauthorized(
            code: Code,
            description: "Invalid credentials");
    }
}
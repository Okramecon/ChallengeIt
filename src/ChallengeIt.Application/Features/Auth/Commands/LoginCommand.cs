using ChallengeIt.Application.Features.Auth.Results;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Application.Utils;
using ChallengeIt.Domain.Entities;
using MediatR;
using ErrorOr;

namespace ChallengeIt.Application.Features.Auth.Commands;

public record LoginCommand(string Username, string Email, string Password) : IRequest<ErrorOr<LoginResult>>;

public class LoginCommandHandler(
    IPasswordHasher passwordHasher,
    IUsersRepository usersRepository,
    ITokenProvider tokenProvider,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<LoginCommand, ErrorOr<LoginResult>>
{
    public async Task<ErrorOr<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        User? user;
        
        if (!string.IsNullOrEmpty(request.Username))
        {
            user = await usersRepository.GetByUserNameAsync(request.Username, cancellationToken);
            
            if (user is null)
                return Error.NotFound($"User {request.Email} was not found");
        }

        user = await usersRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (user is null)
            return Error.NotFound($"User {request.Email} was not found");
        
        if (!passwordHasher.Verify(user.PasswordHash, request.Password))
            return Error.Forbidden("Invalid password");
        
        var accessToken = tokenProvider.GenerateJwtToken(user.Id, user.Username, user.Email);

        var refreshToken = new RefreshToken()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiresOnUtc = dateTimeProvider.UtcNow.AddDays(7)
        };

        await usersRepository.UpdateRefreshTokenAsync(refreshToken, cancellationToken);
        
        return new LoginResult(accessToken, refreshToken.Token);
    }
}
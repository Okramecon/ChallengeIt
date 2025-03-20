using ChallengeIt.Application.Features.Auth.Results;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Domain.Entities;
using MediatR;
using ErrorOr;

namespace ChallengeIt.Application.Features.Auth.Commands;

public record LoginCommand(string Username, string Email, string Password) : IRequest<ErrorOr<LoginResult>>;

public class LoginCommandHandler(
    IPasswordHasher passwordHasher,
    IUsersRepository usersRepository,
    ITokenProvider tokenProvider)
    : IRequestHandler<LoginCommand, ErrorOr<LoginResult>>
{
    public async Task<ErrorOr<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        User? user = null;
        
        if (!string.IsNullOrEmpty(request.Username))
        {
            user = await usersRepository.GetByUserNameAsync(request.Username, cancellationToken);
            
            if (user is null)
                return Error.NotFound($"User {request.Email} was not found");
        }
        else if (!string.IsNullOrEmpty(request.Email))
        {
            user = await usersRepository.GetByEmailAsync(request.Email, cancellationToken);
            
            if (user is null)
                return Error.NotFound($"User {request.Email} was not found");
        }

        if (user is null)
            return Error.Validation("Login", "Login model has to provide username or email");
        
        if (user.IsExternal)
            return Error.Unauthorized("Login", "User is external and cannot login with password");

        if (!passwordHasher.Verify(user.PasswordHash!, request.Password))
            return Error.Unauthorized("Login","Invalid credentials");
        
        var accessToken = tokenProvider.GenerateJwtToken(user.Id, user.Username, user.Email);
        var (refreshTokenValue, refreshExpiresAt) = tokenProvider.GenerateRefreshToken(); 
        
        var refreshToken = new RefreshToken()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = refreshExpiresAt
        };

        await usersRepository.UpdateRefreshTokenAsync(refreshToken, cancellationToken);
        
        return new LoginResult(accessToken, refreshToken.Token);
    }
}
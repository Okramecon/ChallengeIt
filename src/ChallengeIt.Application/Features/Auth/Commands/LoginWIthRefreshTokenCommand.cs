using ChallengeIt.Application.Features.Auth.Results;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Application.Utils;
using MediatR;
using ErrorOr;

namespace ChallengeIt.Application.Features.Auth.Commands;

public record LoginWIthRefreshTokenCommand(string RefreshToken) : IRequest<ErrorOr<LoginResult>>;

public class LoginWIthRefreshTokenCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IUsersRepository usersRepository,
    ITokenProvider tokenProvider
    ) : IRequestHandler<LoginWIthRefreshTokenCommand, ErrorOr<LoginResult>>
{
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IUsersRepository _usersRepository = usersRepository;
    private readonly ITokenProvider _tokenProvider = tokenProvider;

    public async Task<ErrorOr<LoginResult>> Handle(LoginWIthRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _usersRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);
        
        if (refreshToken is null || refreshToken.ExpiresAt < _dateTimeProvider.UtcNow)
            return Error.Unauthorized("Invalid refresh token or token has expired");
        
        var user = await _usersRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
        
        if (user is null)
            return Error.NotFound("Not found user associated with provided refresh token");
        
        var accessToken = _tokenProvider.GenerateJwtToken(user.Id, user.Email, user.Username);

        refreshToken.Token = _tokenProvider.GenerateRefreshToken();
        refreshToken.ExpiresAt = _dateTimeProvider.UtcNow.AddDays(7);
        
        await _usersRepository.UpdateRefreshTokenAsync(refreshToken, cancellationToken);
        
        return new LoginResult(accessToken, refreshToken.Token);
    }
}
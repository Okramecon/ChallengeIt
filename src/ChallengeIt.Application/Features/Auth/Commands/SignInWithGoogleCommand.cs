using ChallengeIt.Application.Features.Auth.Results;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Application.Settings;
using ChallengeIt.Application.Utils;
using ChallengeIt.Domain.Entities;
using ErrorOr;
using Google.Apis.Auth;
using MediatR;
using Microsoft.Extensions.Options;

namespace ChallengeIt.Application.Features.Auth.Commands
{
    public record SignInWithGoogleCommand(string IdToken) : IRequest<ErrorOr<LoginResult>>;

    public class SignInWithGoogleCommandHandler(
        IOptions<GoogleSettings> googleOptions,
        IUsersRepository usersRepository,
        IDateTimeProvider dateTimeProvider,
        ITokenProvider tokenProvider) : IRequestHandler<SignInWithGoogleCommand, ErrorOr<LoginResult>>
    {
        private readonly GoogleSettings _googleSettings = googleOptions.Value;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IUsersRepository usersRepository = usersRepository;
        private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

        public async Task<ErrorOr<LoginResult>> Handle(SignInWithGoogleCommand request, CancellationToken cancellationToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_googleSettings.ClientId]
            });
            if (payload is null)
                return Error.Unauthorized("Invalid Google token.");

            var user = await usersRepository.GetByEmailAsync(payload.Email, cancellationToken);

            if (user is null)
            {
                user = new User
                {
                    Email = payload.Email,
                    Username = payload.Name,
                    PasswordHash = null,
                    IsExternal = true,
                    CreatedAt = _dateTimeProvider.UtcNow
                };

                await usersRepository.AddAsync(user, cancellationToken);
            }

            var accessToken = _tokenProvider.GenerateJwtToken(user.Id, user.Username, user.Email);
            var (refreshTokenValue, refreshExpiresAt) = _tokenProvider.GenerateRefreshToken();

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
}

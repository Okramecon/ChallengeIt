using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Auth.Commands
{
    public record struct SignOutCommand() : IRequest<ErrorOr<Success>>;

    public class SignOutCommandHandler(
        IUsersRepository usersRepository,
        ICurrentUserProvider currentUserProvider) : IRequestHandler<SignOutCommand, ErrorOr<Success>>
    {
        public async Task<ErrorOr<Success>> Handle(SignOutCommand _, CancellationToken cancellationToken)
        {
            var refreshTokenId = currentUserProvider.GetRefreshTokenId();

            if (refreshTokenId.HasValue && refreshTokenId.Value != default)
            {
                await usersRepository.RemoveRefreshTokenAsync(refreshTokenId.Value, cancellationToken);
                return new Success();
            }

            var userId = currentUserProvider.GetUserId();
            await usersRepository.RemoveRefreshTokenAsync(userId, cancellationToken);
            return new Success();
        }
    }
}

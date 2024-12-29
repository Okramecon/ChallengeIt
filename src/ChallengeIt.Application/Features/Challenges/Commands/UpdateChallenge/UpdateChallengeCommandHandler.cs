using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Domain.Errors;
using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Challenges.Commands.UpdateChallenge;

public class UpdateChallengeCommandHandler(
    IChallengesRepository challengesRepository,
    ICurrentUserProvider currentUserProvider)
    : IRequestHandler<UpdateChallengeCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateChallengeCommand request, CancellationToken cancellationToken)
    {
        var challenge = await challengesRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (challenge == null)
            return Error.NotFound(description: ApplicationErrors.ResourceNotFound);

        var userId = currentUserProvider.GetUserId();
        
        if (challenge.UserId != userId)
            return Error.Forbidden(description: ApplicationErrors.ForbiddenAccessToChallenge);
        
        challenge.Title = request.Title;
        
        await challengesRepository.UpdateAsync(challenge, cancellationToken);

        return Result.Success;
    }
}
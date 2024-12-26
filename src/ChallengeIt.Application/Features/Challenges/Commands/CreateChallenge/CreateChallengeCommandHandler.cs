using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Application.Utils;
using ChallengeIt.Domain.Entities;
using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Challenges.Commands.CreateChallenge;

public class CreateChallengeCommandHandler(
    IDateTimeProvider dateTimeProvider,
    ICurrentUserProvider currentUserProvider,
    IChallengesRepository challengesRepository)
    : IRequestHandler<CreateChallengeCommand, ErrorOr<Guid>>
{
    private readonly ICurrentUserProvider _currentUserProvider = currentUserProvider;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IChallengesRepository _challengesRepository = challengesRepository;
    
    public async Task<ErrorOr<Guid>> Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
    {
        var challenge = request.MapToEntity();
        challenge.UserId = _currentUserProvider.GetUserId();
        challenge.CreatedAt = _dateTimeProvider.UtcNow;
        challenge.Status = ChallengeStatus.New;

        return await _challengesRepository.CreateAsync(challenge, cancellationToken);
    }
}
using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Application.Utils;
using ChallengeIt.Domain.Entities;
using MediatR;
using ErrorOr;

namespace ChallengeIt.Application.Features.Challenges.Commands;

public class CreateChallengeCommand : IRequest<ErrorOr<Guid>>
{
    public Challenge MapToEntity() => new Challenge {
        Title = Title,
        BetAmount = BetAmount,
        StartDate = StartDate,
        EndDate = EndDate
    };

    public string Title { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal BetAmount { get; init; }
}
    
public class CreateChallengeCommandHandler(
    IDateTimeProvider dateTimeProvider,
    ICurrentUserProvider currentUserProvider,
    IChallengesRepository featuresRepository)
    : IRequestHandler<CreateChallengeCommand, ErrorOr<Guid>>
{
    private readonly ICurrentUserProvider _currentUserProvider = currentUserProvider;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IChallengesRepository _featuresRepository = featuresRepository;
    
    public async Task<ErrorOr<Guid>> Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
    {
        var challenge = request.MapToEntity();
        challenge.UserId = _currentUserProvider.GetUserId();
        challenge.CreatedAt = _dateTimeProvider.UtcNow;
        challenge.Status = ChallengeStatus.New;

        return await _featuresRepository.CreateAsync(challenge);
    }
}

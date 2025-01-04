using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Application.Utils;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Errors;
using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Challenges.Commands.CheckInChallengeDay;

public class CheckInChallengeDayCommandHandler(
    IChallengesRepository challengesRepository,
    ICheckInsRepository checkInsRepository,
    ICurrentUserProvider userProvider,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CheckInChallengeDayCommand, ErrorOr<Success>>
{
    private readonly IChallengesRepository _challengesRepository = challengesRepository;
    private readonly ICheckInsRepository _checkInsRepository = checkInsRepository;
    private readonly ICurrentUserProvider _userProvider = userProvider;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public async Task<ErrorOr<Success>> Handle(CheckInChallengeDayCommand request, CancellationToken cancellationToken)
    {
        var userId = _userProvider.GetUserId();
        
        var challenge = await _challengesRepository.GetByIdAsync(request.ChallengeId, cancellationToken);
        
        if (challenge is null)
            return Error.NotFound(description: ApplicationErrors.ResourceNotFound);
        
        if (challenge.UserId != userId)
            return Error.Forbidden("You are not authorized to check in this challenge.");
        
        var currentDate = _dateTimeProvider.UtcNow;

        if (!challenge.IsActive(currentDate))
            return Error.Conflict("Challenge is not active.");
        
        var checkInDateEntity = await _checkInsRepository.GetChallengeCheckInAsync(request.ChallengeId, currentDate, cancellationToken);
        if (checkInDateEntity is null)
        {
            checkInDateEntity = new CheckIn
            {
                Checked = true,
                UserId = userId,
                ChallengeId = request.ChallengeId,
                Date = currentDate,
            };
            
            await _checkInsRepository.CreateAsync(checkInDateEntity, null, cancellationToken);
            return Result.Success;
        }
        
        await _checkInsRepository.CheckInChallengeDate(checkInDateEntity.Id, cancellationToken);
        return Result.Success;
    }
}
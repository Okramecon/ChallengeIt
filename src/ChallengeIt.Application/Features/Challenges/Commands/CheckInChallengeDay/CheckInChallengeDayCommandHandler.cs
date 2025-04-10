using System;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Application.Utils;
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
    public async Task<ErrorOr<Success>> Handle(CheckInChallengeDayCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.CheckInId.HasValue)
            {
                return await CheckInWithCheckInIdAsync(request.CheckInId.Value, cancellationToken);
            }

            if (request.ChallengeId.HasValue)
            {
                return await CheckInWithChallengeIdAsync(request.ChallengeId.Value, cancellationToken);
            }

            return Error.Validation("Invalid request", "Either CheckInId or ChallengeId must be provided.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Error.Failure(description: "Error occured while update check-in day.");
        }
    }

    private async Task<ErrorOr<Success>> CheckInWithChallengeIdAsync(Guid challengeId, CancellationToken cancellationToken)
    {
        var challenge = await challengesRepository.GetByIdAsync(challengeId, cancellationToken);
        if (challenge is null)
        {
            return Error.NotFound(description: ApplicationErrors.ResourceNotFound);
        }

        var userId = userProvider.GetUserId();
        if (challenge.UserId != userId)
        {
            return Error.Forbidden(description: "You are not authorized to check in this challenge.");
        }

        var currentDate = dateTimeProvider.UtcNow.Date;
        if (!challenge.IsActive(currentDate))
        {
            return Error.Conflict(description: "Challenge is not active.");
        }

        var checkInEntity = await checkInsRepository.GetChallengeCheckInAsync(challengeId, currentDate, cancellationToken);
        if (checkInEntity is null)
        {
            return Error.NotFound(description: ApplicationErrors.ResourceNotFound);
        }

        await checkInsRepository.CheckInChallengeDate(checkInEntity.Id, cancellationToken);
        return Result.Success;
    }

    /// <summary>
    /// Only in case when challenge is active
    /// </summary>
    /// <param name="checkInId">Check in identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Error or Success</returns>
    private async Task<ErrorOr<Success>> CheckInWithCheckInIdAsync(Guid checkInId, CancellationToken cancellationToken)
    {
        var checkInEntity = await checkInsRepository.GetByIdAsync(checkInId, cancellationToken);
        if (checkInEntity is null)
        {
            return Error.NotFound(description: ApplicationErrors.ResourceNotFound);
        }

        var userId = userProvider.GetUserId();
        if (checkInEntity.UserId != userId)
        {
            return Error.Forbidden(description: "You are not authorized to check in this challenge.");
        }

        if (checkInEntity.Date.Date != dateTimeProvider.UtcNow.Date)
        {
            return Error.Conflict(description : "You cannot check in this date.");
        }

        await checkInsRepository.CheckInChallengeDate(checkInId, cancellationToken);
        return Result.Success;
    }
}
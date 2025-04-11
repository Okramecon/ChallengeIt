using ChallengeIt.API.Contracts.Challenges;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Application.Utils;
using ChallengeIt.Domain.Entities;
using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Challenges.Commands.CreateChallenge;

public class CreateChallengeCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork,
    ICurrentUserProvider currentUserProvider)
    : IRequestHandler<CreateChallengeCommand, ErrorOr<CreateChallengeResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ICurrentUserProvider _currentUserProvider = currentUserProvider;
    
    public async Task<ErrorOr<CreateChallengeResponse>> Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.GetUserId();
        
        var challenge = request.MapToEntity();
        challenge.UserId = userId;
        challenge.CreatedAt = _dateTimeProvider.UtcNow;
        challenge.Status = ChallengeStatus.New;
        
        List<DateTime> checkInDates = request.Everyday
            ? [.. Enumerable.Range(0, (int)(request.EndDate - request.StartDate).TotalDays + 1).Select(i => request.StartDate.AddDays(i))]
            : [.. request.Schedule!.Distinct()];
        
        try
        {
            _unitOfWork.BeginTransaction();

            // Insert Challenge
            var challengeId = await _unitOfWork.Challenges.CreateAsync(challenge, null, cancellationToken);

            if (challengeId == Guid.Empty)
            {
                _unitOfWork.Rollback();
                return Error.Unexpected("Error creating challenge.");
            }

            // Insert Check-Ins
            var checkIns = CreateCheckIns(challengeId, userId, checkInDates).ToList();
            checkIns.Last().IsLast = true;

            var createdCheckIns = await _unitOfWork.CheckIns.CreateBatchAsync(checkIns, cancellationToken);
                
            _unitOfWork.Commit();

            return new CreateChallengeResponse(
                challengeId,
                challenge.Title,
                createdCheckIns.Select(x => new CreateCheckInModel(
                    x.Id,
                    x.Date)).ToList()
                );
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Error.Unexpected($"Transaction failed: {ex.Message}");
        }
        finally
        {
            _unitOfWork.ConnectionClose();
        }
    }

    private static IEnumerable<CheckIn> CreateCheckIns(Guid challengeId, long userId, List<DateTime> dates) => dates
        .Distinct()
        .OrderBy(d => d.Date)
        .Select(d => new CheckIn
        {
            Id = Guid.NewGuid(),
            ChallengeId = challengeId,
            UserId = userId,
            Date = d,
            Checked = false
        });
}
using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Models;

namespace ChallengeIt.Application.Persistence;

public interface ICheckInsRepository : IBaseCrudRepository<CheckIn, Guid>
{
    Task<List<CheckIn>> GetChallengeCheckInsAsync(Guid challengeId, CancellationToken cancellationToken = default);
    Task<CheckIn?> GetChallengeCheckInAsync(Guid challengeId, DateTime dateTime, CancellationToken cancellationToken = default);
    Task CheckInChallengeDate(Guid id, CancellationToken cancellationToken = default);
    Task<List<TodayCheckInModel>> GetCheckinsForDayAsync(DateTime date, long userId, CancellationToken cancellationToken = default);
}
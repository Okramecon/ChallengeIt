using ChallengeIt.Domain.Entities;

namespace ChallengeIt.Application.Persistence;

public interface ICheckInsRepository : IBaseCrudRepository<CheckIn, Guid>
{
    public Task<List<CheckIn>> GetChallengeCheckInsAsync(Guid challengeId, CancellationToken cancellationToken = default);
    public Task<CheckIn?> GetChallengeCheckInAsync(Guid challengeId, DateTime dateTime, CancellationToken cancellationToken = default);
    public Task CheckInChallengeDate(Guid id, CancellationToken cancellationToken = default);
}
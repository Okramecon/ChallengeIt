using ChallengeIt.Application.Persistence;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Infrastructure.Persistence.Dapper;

namespace ChallengeIt.Infrastructure.Persistence.Repositories;

public class CheckInsRepository(ISqlDbContext context)
    : BaseCrudRepository<CheckIn, Guid>(context, "checkins"), ICheckInsRepository
{
    public Task<List<CheckIn>> GetChallengeCheckInsAsync(Guid challengeId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CheckIn?> GetChallengeCheckInAsync(Guid challengeId, DateTime dateTime, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task CheckInChallengeDate(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
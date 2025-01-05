using ChallengeIt.Application.Persistence;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Infrastructure.Persistence.Dapper;
using Dapper;

namespace ChallengeIt.Infrastructure.Persistence.Repositories;

public class CheckInsRepository(ISqlDbContext context)
    : BaseCrudRepository<CheckIn, Guid>(context, "checkins"), ICheckInsRepository
{
    public Task<List<CheckIn>> GetChallengeCheckInsAsync(Guid challengeId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private const string GetChallengeCheckInQuery = @"
    SELECT id 
    FROM checkins 
    WHERE challenge_id = @id::uuid AND date = @date::date;
";

    public async Task<CheckIn?> GetChallengeCheckInAsync(Guid challengeId, DateTime date, CancellationToken cancellationToken = default)
    {
        // Ensure the time component is stripped from the DateTime
        var truncatedDate = date.Date;

        return await DbConnection.QueryFirstOrDefaultAsync<CheckIn>(
            GetChallengeCheckInQuery,
            new { id = challengeId, date = truncatedDate });
    }

    private const string CheckInChallengeDayQuery =
    """
        UPDATE checkins SET checked = true WHERE id = @id::uuid;
    """;
    
    public async Task CheckInChallengeDate(Guid id, CancellationToken cancellationToken = default)
    {
        await DbConnection.ExecuteAsync(CheckInChallengeDayQuery, new { id });
    }
}
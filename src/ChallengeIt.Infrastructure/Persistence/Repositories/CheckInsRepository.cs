using ChallengeIt.Application.Persistence;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Models;
using ChallengeIt.Infrastructure.Persistence.Dapper;
using Dapper;
using Microsoft.EntityFrameworkCore;

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

    private const string GetCheckinsForDayQuery = @"
        SELECT c.id AS ""Id"", c.date AS ""Date"", c.checked AS ""Checked"", c.challenge_id AS ""ChallengeId"", c.user_id AS ""UserId"", c0.title AS ""ChallengeTitle""
        FROM checkins as c
        INNER JOIN challenges AS c0 ON c.challenge_id = c0.id
        WHERE c.user_id = @id AND c.date = @date::date AND c.failed_challenge = false;
    ";

    public async Task<List<TodayCheckInModel>> GetCheckinsForDateAsync(DateTime date, long userId, CancellationToken cancellationToken = default)
    {
        return [.. (await DbConnection.QueryAsync<TodayCheckInModel>(GetCheckinsForDayQuery, new
        {
            id = userId,
            date = date
        }))];
    }

    private const string GetCheckinsForDateQuery = @"
        SELECT c.id AS ""Id"", c.challenge_id AS ""ChallengeId""
        FROM checkins as c
        INNER JOIN challenges AS c0 ON c.challenge_id = c0.id
        WHERE c.time_zone = @timeZoneId AND c.date = @date::date AND c.checked = FALSE;
    ";

    public async Task<List<CheckIn>> GetUncheckedCheckInsForDateAsync(string timeZoneId, DateTime date, CancellationToken cancellationToken = default)
    {
        return [.. (await DbConnection.QueryAsync<CheckIn>(GetCheckinsForDateQuery, new
        {
            timeZoneId,
            date
        }))];
    }
}
using System.Data;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Models.Paging;
using ChallengeIt.Infrastructure.Persistence.Dapper;
using Dapper;
using static Dapper.SqlMapper;

namespace ChallengeIt.Infrastructure.Persistence.Repositories;

public class ChallengesRepository(ISqlDbContext sqlDbContext)
    : BaseCrudRepository<Challenge, Guid>(sqlDbContext, "challenges"), IChallengesRepository
{
    public async Task<Page<Challenge>> GetUserChallengesAsync(
        PageRequest pageRequest,
        long userId,
        CancellationToken cancellationToken = default)
    {
        string query = $"SELECT * FROM challenges WHERE user_id = @UserId OFFSET @Offset LIMIT @PageSize";

        var items = await DbConnection.QueryAsync<Challenge>(query, new
        {
            Offset = (pageRequest.PageNumber - 1) * pageRequest.PageSize,
            pageRequest.PageSize,
            UserId = userId
        });

        string countQuery = $"SELECT COUNT(*) FROM challenges WHERE user_id = @UserId";
        int totalItems =
            await DbConnection.ExecuteScalarAsync<int>(countQuery, new { UserId = userId },
                commandType: CommandType.Text);

        return new Page<Challenge>
        (
            Items: items.ToArray(),
            TotalCount: totalItems,
            PageNumber: pageRequest.PageNumber,
            PageSize: pageRequest.PageSize
        );
    }


    const string UpdateMissedDaysCountQuery = $"UPDATE challenges SET missed_days_count = @missedDayCount where id = @id::uuid";
    const string UpdateStatusToFailedQuery = $"UPDATE challenges SET missed_days_count = @missedDayCount, status = @status where id = @id::uuid";
    const string MarkAllChallengeDaysAsFailed = $"UPDATE checkins SET failed_challenge = true where challenge_id = @challengeId::uuid";

    public async Task<bool> ProcessMissedChallengeActivityAsync(Guid challengeId, IDbTransaction? transaction = null)
    {
        var challenge = await GetByIdAsync(challengeId);

        if (challenge is null)
        {
            return false;
        }
        challenge.MissedDaysCount += 1;
        if (challenge.MissedDaysCount > challenge.MaxAllowedMissedDaysCount)
        {
            await DbConnection.ExecuteAsync(UpdateStatusToFailedQuery, new
            {
                id = challengeId,
                missedDayCount = challenge.MissedDaysCount,
                status = nameof(ChallengeStatus.Failed)
            });

            await DbConnection.ExecuteAsync(MarkAllChallengeDaysAsFailed, new
            {
                challengeId,
            });

            return true;
        }

        await DbConnection.ExecuteAsync(UpdateMissedDaysCountQuery, new
        {
            id = challengeId,
            missedDayCount = challenge.MissedDaysCount
        });

        return true;

    }

    const string UpdateChallengeStatusQuery = $"UPDATE challenges SET status = @status where id = @id::uuid";

    public async Task UpdateChallengeStatusAsync(Guid challengeId, ChallengeStatus status)
    {
        await DbConnection.QuerySingleAsync(UpdateChallengeStatusQuery, new
        {
            id = challengeId,
            status = status.ToString()
        });
    }
}
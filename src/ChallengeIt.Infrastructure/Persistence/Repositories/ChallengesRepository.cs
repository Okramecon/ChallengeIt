using System.Data;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Models.Paging;
using ChallengeIt.Infrastructure.Persistence.Dapper;
using Dapper;

namespace ChallengeIt.Infrastructure.Persistence.Repositories;

public class ChallengesRepository(IDapperContext context)
    : BaseCrudRepository<Challenge, Guid>(context, "challenges"), IChallengesRepository
{
    private readonly IDapperContext _context = context;

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
    
    private const string CreateChallengeQuery =
        """
        INSERT INTO challenges (id, user_id, title, bet_amount, start_date, end_date, missed_days_count,
                                max_allowed_missed_days, status, created_at)
        VALUES (@Id, @UserId, @Title, @BetAmount, @StartDate, @EndDate, @MissedDaysCount, @MaxAllowedMissedDaysCount, @Status, @CreatedAt)
        RETURNING id
        """;
    
    public async Task<Guid> CreateAsync(Challenge challenge)
    {
        using var connection = _context.CreateConnection();
        connection.Open();
        return await connection.QuerySingleAsync<Guid>(CreateChallengeQuery, challenge);
    }
}
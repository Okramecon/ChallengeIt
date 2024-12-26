using ChallengeIt.Application.Persistence;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Infrastructure.Persistence.Dapper;
using Dapper;

namespace ChallengeIt.Infrastructure.Persistence.Repositories;

public class ChallengesRepository : BaseCrudRepository<Challenge, Guid>, IChallengesRepository
{
    private readonly IDapperContext _context;

    public ChallengesRepository(IDapperContext context) : base(context, "challenges")
    {
        _context = context;
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
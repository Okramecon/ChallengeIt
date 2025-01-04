using System.Data;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Models.Paging;
using ChallengeIt.Infrastructure.Persistence.Dapper;
using Dapper;

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
}
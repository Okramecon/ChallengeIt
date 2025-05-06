using System.Data;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Models.Paging;

namespace ChallengeIt.Application.Persistence;

public interface IChallengesRepository : IBaseCrudRepository<Challenge, Guid>
{
    Task<Page<Challenge>> GetUserChallengesAsync(PageRequest pageRequest, long userId, CancellationToken cancellationToken = default);
    Task<bool> ProcessMissedChallengeActivityAsync(Guid challengeId, IDbTransaction? transaction = null);
    Task UpdateChallengeStatusAsync(Guid challengeId, ChallengeStatus status);
}
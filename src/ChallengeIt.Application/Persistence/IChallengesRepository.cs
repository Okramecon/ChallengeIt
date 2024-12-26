using ChallengeIt.Domain.Entities;

namespace ChallengeIt.Application.Persistence;

public interface IChallengesRepository : IBaseCrudRepository<Challenge, Guid>
{
}
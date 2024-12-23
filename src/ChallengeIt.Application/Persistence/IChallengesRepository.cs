using ChallengeIt.Domain.Entities;

namespace ChallengeIt.Application.Persistence;

public interface IChallengesRepository
{
    Task<Guid> CreateAsync(Challenge challenge);
}
using ChallengeIt.Domain.Entities;

namespace ChallengeIt.Application.Persistence;

public interface IUsersRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> IsUsedEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsUsedUsernameAsync(string userName, CancellationToken cancellationToken = default);
}
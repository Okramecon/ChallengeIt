using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Models.User;

namespace ChallengeIt.Application.Persistence;

public interface IUsersRepository
{
    Task<long> AddAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(long userId, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> IsUsedEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsUsedUsernameAsync(string userName, CancellationToken cancellationToken = default);
    Task UpdateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<List<SearchUserProfileModel>> FindUsersByNameAsync(string userName,
        CancellationToken cancellationToken = default);
}
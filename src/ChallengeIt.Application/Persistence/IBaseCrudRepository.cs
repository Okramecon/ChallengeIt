using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Models.Paging;

namespace ChallengeIt.Application.Persistence;

public interface IBaseCrudRepository<TEntity, TKey> 
    where TEntity : Entity<TKey> 
    where TKey : struct
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<Page<TEntity>?> GetPageAsync(PageRequest page, CancellationToken cancellationToken = default);
    Task<TKey> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}
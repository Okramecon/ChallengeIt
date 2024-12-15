using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Models.Paging;

namespace ChallengeIt.Application.Persistence;

public interface IBaseCrudRepository<TEntity, in TKey> 
    where TEntity : Entity<TKey> 
    where TKey : struct
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken);
    Task<Page<TEntity>?> GetPageAsync(PageRequest page, CancellationToken cancellationToken);
    Task<TEntity?> CreateAsync(TEntity entity, CancellationToken cancellationToken);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    Task DeleteAsync(TKey id, CancellationToken cancellationToken);
}
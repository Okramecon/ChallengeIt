using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Models.Paging;
using ChallengeIt.Infrastructure.Persistence.Dapper;
using Dapper;

namespace ChallengeIt.Infrastructure.Persistence.Repositories;

public class BaseCrudRepository<TEntity, TKey>(ISqlDbContext context, string tableName)
    : IBaseCrudRepository<TEntity, TKey>
    where TEntity : Entity<TKey>
    where TKey : struct
{
    protected readonly IDbConnection DbConnection = context.CurrentConnection;
    protected IDbTransaction? DbTransaction => context.CurrentTransaction; // it is important to be a property
    
    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        string query = $"SELECT * FROM \"{tableName}\" WHERE \"id\" = @Id";
        return await DbConnection.QueryFirstOrDefaultAsync<TEntity>(query, new { Id = id });
    }

    public async Task<Page<TEntity>> GetPageAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        string query = $"SELECT * FROM \"{tableName}\"  OFFSET @Offset LIMIT @PageSize";

        var items = await DbConnection.QueryAsync<TEntity>(query, new
        {
            Offset = (pageRequest.PageNumber - 1) * pageRequest.PageSize,
            pageRequest.PageSize
        });

        string countQuery = $"SELECT COUNT(*) FROM \"{tableName}\"";
        int totalItems = await DbConnection.ExecuteScalarAsync<int>(countQuery);

        return new Page<TEntity>
        (
            Items: items.ToArray(),
            TotalCount: totalItems,
            PageNumber: pageRequest.PageNumber,
            PageSize: pageRequest.PageSize
        );
    }

    public async Task<TKey> CreateAsync(TEntity entity, IDbTransaction? tr = default, CancellationToken cancellationToken = default)
    {
        Debug.Assert(DbTransaction?.Connection == DbConnection, "Transaction must be associated with the same connection.");
        
        string query = $"INSERT INTO \"{tableName}\" ({GetColumns(true)}) VALUES ({GetValues(true)}) RETURNING \"id\"";

        var id = await DbConnection.QuerySingleAsync<TKey>(query, entity, transaction: tr);
        return id;
    }
    
    public async Task<List<TEntity>> CreateBatchAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        if (entities == null || !entities.Any())
        {
            throw new ArgumentException("The entities list cannot be null or empty.", nameof(entities));
        }

        // Build the insert query for the batch
        string query = $"INSERT INTO \"{tableName}\" ({GetColumns(true)}) VALUES {GetValuesBatch(entities.Count, true)} RETURNING \"id\"";

        // Flatten the entity properties into a single anonymous object for parameter binding
        var parameters = new DynamicParameters();
        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];
            foreach (var property in typeof(TEntity).GetProperties())
            {
                var paramName = $"{property.Name}_{i}";
                parameters.Add(paramName, property.GetValue(entity));
            }
        }

        // Execute the batch insert query
        var insertedIds = await DbConnection.QueryAsync<TKey>(query, parameters, transaction: DbTransaction);

        // Assign the IDs back to the entities and return them
        var insertedEntities = entities.Zip(insertedIds, (entity, id) =>
        {
            typeof(TEntity).GetProperty("Id")?.SetValue(entity, id);
            return entity;
        }).ToList();

        return insertedEntities;
    }


    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        string query = $"UPDATE \"{tableName}\" SET {GetUpdateSetClause()} WHERE \"id\" = @Id";
        await DbConnection.ExecuteAsync(query, entity, transaction: DbTransaction);
    }

    public async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        string query = $"DELETE FROM \"{tableName}\" WHERE \"id\" = @Id";
        await DbConnection.ExecuteAsync(query, new { Id = id }, transaction: DbTransaction);
    }

    private string GetColumns(bool includeIdentity = false)
    {
        var properties = typeof(TEntity).GetProperties()
            .Where(p => includeIdentity || p.Name != "Id")
            .Select(GetColumnName);
        return string.Join(", ", properties);
    }

    private string GetValues(bool includeIdentity = false)
    {
        var properties = typeof(TEntity).GetProperties()
            .Where(p => includeIdentity || p.Name != "Id")
            .Select(p => "@" + p.Name);
        return string.Join(", ", properties);
    }
    
    private string GetValuesBatch(int count, bool includeIdentity = false)
    {
        var properties = typeof(TEntity).GetProperties()
            .Where(p => includeIdentity || p.Name != "Id")
            .ToList();

        var valueGroups = Enumerable.Range(0, count)
            .Select(i => $"({string.Join(", ", properties.Select(p => $"@{p.Name}_{i}"))})");

        return string.Join(", ", valueGroups);
    }

    private string GetUpdateSetClause(bool includeIdentity = false)
    {
        var properties = typeof(TEntity).GetProperties()
            .Where(p => includeIdentity || p.Name != "Id")
            .Select(p => $"{GetColumnName(p)} = @{p.Name}");
        
        return string.Join(", ", properties);
    }

    private string GetColumnName(PropertyInfo property)
    {
        var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
        return columnAttribute != null ? $"\"{columnAttribute.Name}\"" : $"\"{property.Name}\"";
    }
}
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Models.Paging;
using ChallengeIt.Infrastructure.Persistence.Dapper;
using Dapper;

namespace ChallengeIt.Infrastructure.Persistence.Repositories;

public class BaseCrudRepository<TEntity, TKey>(IDapperContext context, string tableName)
    : IBaseCrudRepository<TEntity, TKey>
    where TEntity : Entity<TKey>
    where TKey : struct
{
    private readonly IDbConnection _dbConnection = context.CreateConnection();

    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        string query = $"SELECT * FROM \"{tableName}\" WHERE \"id\" = @Id";
        return await _dbConnection.QueryFirstOrDefaultAsync<TEntity>(query, new { Id = id });
    }

    public async Task<Page<TEntity>?> GetPageAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        string query = $"SELECT * FROM \"{tableName}\"  OFFSET @Offset LIMIT @PageSize";

        var items = await _dbConnection.QueryAsync<TEntity>(query, new
        {
            Offset = (pageRequest.PageNumber - 1) * pageRequest.PageSize,
            PageSize = pageRequest.PageSize
        });

        string countQuery = $"SELECT COUNT(*) FROM \"{tableName}\"";
        int totalItems = await _dbConnection.ExecuteScalarAsync<int>(countQuery);

        return new Page<TEntity>
        (
            Items: items.ToArray(),
            TotalCount: totalItems,
            PageNumber: pageRequest.PageNumber,
            PageSize: pageRequest.PageSize
        );
    }

    public async Task<TKey> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        string query = $"INSERT INTO \"{tableName}\" ({GetColumns(true)}) VALUES ({GetValues(true)}) RETURNING \"id\"";

        var id = await _dbConnection.QuerySingleAsync<TKey>(query, entity);
        return id;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        string query = $"UPDATE \"{tableName}\" SET {GetUpdateSetClause()} WHERE \"id\" = @Id";
        await _dbConnection.ExecuteAsync(query, entity);
    }

    public async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        string query = $"DELETE FROM \"{tableName}\" WHERE \"id\" = @Id";
        await _dbConnection.ExecuteAsync(query, new { Id = id });
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
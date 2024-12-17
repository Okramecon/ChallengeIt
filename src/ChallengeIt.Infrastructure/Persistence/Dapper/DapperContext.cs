using System.Data;
using Dapper;
using Npgsql;

namespace ChallengeIt.Infrastructure.Persistence.Dapper;

public class DapperContext : IDapperContext
{
    private readonly string _connectionString;

    public DapperContext(DapperContextOptions options)
    {
        if (options is null || string.IsNullOrWhiteSpace(options.ConnectionString))
            throw new ArgumentNullException(nameof(options.ConnectionString));
        
        _connectionString = options.ConnectionString;
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
     
    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
    public IDbConnection CreateConnection(string connectionString) => new NpgsqlConnection(connectionString);

    public IDbTransaction CreateTransaction()
    {
        var connection = CreateConnection();
        connection.Open();
        return connection.BeginTransaction();
    }

}
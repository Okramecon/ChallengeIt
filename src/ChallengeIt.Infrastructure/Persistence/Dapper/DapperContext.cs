using System.Data;
using Dapper;
using Npgsql;

namespace ChallengeIt.Infrastructure.Persistence.Dapper;

/// <summary>
/// Unit of work Dapper implementation
/// </summary>
public class DapperContext : ISqlDbContext
{
    public DapperContext(DapperContextOptions options)
    {
        if (options is null || string.IsNullOrWhiteSpace(options.ConnectionString))
            throw new ArgumentNullException(nameof(options.ConnectionString));
        
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        CurrentConnection = new NpgsqlConnection(options.ConnectionString);
    }

    public IDbConnection CurrentConnection { get; private set; }
    public IDbTransaction? CurrentTransaction { get; private set; }
    
    public IDbTransaction BeginTransaction()
    {
        if (CurrentConnection.State != ConnectionState.Open)
        {
            CurrentConnection.Open();
        }
        
        return CurrentTransaction = CurrentConnection.BeginTransaction();
    }
    
    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
    {
        if (CurrentConnection.State != ConnectionState.Open)
        {
            CurrentConnection.Open();
        }
        
        return CurrentTransaction = CurrentConnection.BeginTransaction(isolationLevel);
    }

    public void ConnectionOpen()
    {
        if (CurrentConnection.State != ConnectionState.Open)
            CurrentConnection.Open();
    } 
    
    public void ConnectionClose()
    {
        if (CurrentConnection.State != ConnectionState.Closed)
            CurrentConnection.Close();
    }
    
    public void Dispose()
    {
        if (CurrentConnection.State == ConnectionState.Open)
        {
            CurrentConnection.Close();
        }
        CurrentConnection.Dispose();
    }
}
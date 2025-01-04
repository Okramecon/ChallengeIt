using System.Data;

namespace ChallengeIt.Infrastructure.Persistence.Dapper;

public interface ISqlDbContext
{
    public IDbConnection CurrentConnection { get; }
    public IDbTransaction? CurrentTransaction { get; }
    
    public IDbTransaction BeginTransaction();
    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel);
}
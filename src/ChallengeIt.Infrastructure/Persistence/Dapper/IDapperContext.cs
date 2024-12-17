using System.Data;

namespace ChallengeIt.Infrastructure.Persistence.Dapper;

public interface IDapperContext
{
    public IDbConnection CreateConnection();
    public IDbConnection CreateConnection(string connectionString);
    public IDbTransaction CreateTransaction();
}
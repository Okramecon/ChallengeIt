using System.Data;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Infrastructure.Persistence.Repositories;

namespace ChallengeIt.Infrastructure.Persistence.Dapper;

public class UnitOfWork(ISqlDbContext sqlDbContext) : IUnitOfWork
{
    public IDbTransaction BeginTransaction()
    {
        if (sqlDbContext.CurrentConnection.State != ConnectionState.Open)
        {
            sqlDbContext.CurrentConnection.Open();
        }

        if (sqlDbContext.CurrentTransaction is not null)
            return sqlDbContext.CurrentTransaction;
        
        return sqlDbContext.BeginTransaction();
    }

    public void ConnectionClose()
    {
        sqlDbContext.CurrentConnection.Close();
    }

    public void ConnectionOpen()
    {
        sqlDbContext.CurrentConnection.Open();
    }

    public void Commit()
    {
        sqlDbContext.CurrentTransaction?.Commit();
        DisposeTransaction();
    }

    public void Rollback()
    {
        try
        {
            sqlDbContext.CurrentTransaction?.Rollback();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        DisposeTransaction();
    }

    private void DisposeTransaction()
    {
        sqlDbContext.CurrentTransaction?.Dispose();
    }
    
    public IChallengesRepository Challenges { get; private set; } = new ChallengesRepository(sqlDbContext);

    public IUsersRepository Users { get; private set; } = new UsersRepository(sqlDbContext);
    public ICheckInsRepository CheckIns { get; private set; } = new CheckInsRepository(sqlDbContext);
}
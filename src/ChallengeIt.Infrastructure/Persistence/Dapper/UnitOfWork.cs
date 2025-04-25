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
    
    private readonly Lazy<ChallengesRepository> _challengesLazy = new(() => new ChallengesRepository(sqlDbContext));
    public IChallengesRepository Challenges => _challengesLazy.Value;

    private readonly Lazy<UsersRepository> _usersLazy = new(() => new UsersRepository(sqlDbContext));
    public IUsersRepository Users => _usersLazy.Value;

    private readonly Lazy<CheckInsRepository> _checkInsLazy = new(() => new CheckInsRepository(sqlDbContext));
    public ICheckInsRepository CheckIns => _checkInsLazy.Value;
}
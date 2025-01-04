using System.Data;

namespace ChallengeIt.Application.Persistence;

public interface IUnitOfWork
{
    IUsersRepository Users { get; }
    IChallengesRepository Challenges { get; }
    ICheckInsRepository CheckIns { get; }
    
    public IDbTransaction BeginTransaction();
    public void ConnectionClose();
    public void ConnectionOpen();
    public void Commit();
    public void Rollback();
}
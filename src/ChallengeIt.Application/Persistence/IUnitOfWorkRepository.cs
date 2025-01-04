using System.Data;

namespace ChallengeIt.Application.Persistence;

public interface IUnitOfWorkRepository
{
    IDbTransaction? Transaction { get; }
    void SetTransaction(ref IDbTransaction? transaction);
}
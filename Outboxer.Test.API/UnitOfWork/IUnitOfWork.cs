namespace Outboxer.Test.API.UnitOfWork;

public interface IUnitOfWork
{
    Task<int> CommitAsync();
    int Commit();
}
using Outboxer.Test.API.Context;

namespace Outboxer.Test.API.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly StudentsContext _context;

    public UnitOfWork(StudentsContext context)
    {
        _context = context;
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public int Commit()
    {
        return _context.SaveChanges();
    }
}
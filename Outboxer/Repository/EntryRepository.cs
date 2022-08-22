using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Outboxer.Context;
using Outboxer.Enums;
using Outboxer.Models;

namespace Outboxer.Repository;

internal class EntryRepository<TContext> : IEntryRepository where TContext : OutboxerContext<TContext>
{
    private readonly DbSet<Entry> _dbSet;

    public EntryRepository(TContext context)
    {
        _dbSet = context.Set<Entry>();
    }

    public async Task Add(Entry entry) => 
        await _dbSet.AddAsync(entry);

    public async Task Add(IEnumerable<Entry> entries) =>
        await _dbSet.AddRangeAsync(entries);

    public void Update(Entry entry) =>
        _dbSet.Update(entry);

    public async Task<Entry> Get(Guid entryId) =>
        await _dbSet.FindAsync(entryId);

    public async Task<List<Entry>> GetEnqueued() =>
        await _dbSet.Where(x => x.Status == StatusEnum.ENQUEUED).ToListAsync();
}
using Outboxer.Models;

namespace Outboxer.Repository;

public interface IEntryRepository
{
    Task Add(Entry entry);
    Task Add(IEnumerable<Entry> entries);
    void Update(Entry entry);
    Task<Entry> Get(Guid entryId);
    Task<List<Entry>> GetEnqueued();
}
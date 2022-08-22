using Outboxer.Models;

namespace Outboxer.Transactional;

public interface ITransactionalMessageContainer 
{
    Task AddToPendingEntries(Entry entry);
    Task AddToPendingEntries(List<Entry> entries);
    Task Dispatch();
}
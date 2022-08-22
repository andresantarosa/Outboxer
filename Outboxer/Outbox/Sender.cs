using Outboxer.Models;
using Outboxer.Repository;
using Outboxer.Transactional;

namespace Outboxer.Outbox;

public class Publisher : IPublisher
{
    private readonly IEntryRepository _repository;
    private readonly ITransactionalMessageContainer _transactionalMessageContainer;

    public Publisher(IEntryRepository repository,
                     ITransactionalMessageContainer transactionalMessageContainer)
    {
        _repository = repository;
        _transactionalMessageContainer = transactionalMessageContainer;
    }

    /// <summary>
    /// Adds entry to Outbox repository and adds it to temporary area
    /// </summary>
    /// <param name="entry"></param>
    public async Task Publish(Entry entry)
    {
        await _repository.Add(entry);
        await _transactionalMessageContainer.AddToPendingEntries(entry);
    }

    /// <summary>
    /// Adds entries to Outbox repository and adds them to temporary area
    /// </summary>
    /// <param name="entry"></param>
    public async Task Publish(List<Entry> entries)
    {
        await _repository.Add(entries);
        await _transactionalMessageContainer.AddToPendingEntries(entries);
    }

    /// <summary>
    /// Set an entry as delivered at database
    /// </summary>
    /// <param name="entry"></param>
    public void SetDelivered(Entry entry)
    {
        entry.SetDelivered();
        _repository.Update(entry);
    }

    /// <summary>
    /// Set an entry as failed at database
    /// </summary>
    /// <param name="entry"></param>
    public void SetFailure(Entry entry)
    {
        entry.IncreaseRetryCount();
        _repository.Update(entry);
    }
}
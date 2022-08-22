using Outboxer.Channels;
using Outboxer.Models;

namespace Outboxer.Transactional;

/// <summary>
/// Works like a temporary area where the messages will be saved until the commit happens
/// </summary>
internal class TransactionalMessageContainer : ITransactionalMessageContainer
{
    private readonly IOutboxChannel _outboxChannel;

    public TransactionalMessageContainer(IOutboxChannel outboxChannel)
    {
        _outboxChannel = outboxChannel;
    }
    
    /// <summary>
    /// Property that hold the messages until they be sent to Channel
    /// </summary>
    private List<Entry> Entries { get; } = new();

    public async Task AddToPendingEntries(Entry entry) =>
        Entries.Add(entry);
    
    public async Task AddToPendingEntries(List<Entry> entries) =>
        Entries.AddRange(entries);
    
    /// <summary>
    /// Insert messages into Channel to be sent to broker
    /// </summary>
    public async Task Dispatch()
    {
        foreach (var entry in Entries) 
            await _outboxChannel.Enqueue(entry.Id);
    }
}
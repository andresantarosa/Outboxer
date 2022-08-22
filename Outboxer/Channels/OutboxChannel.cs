using System.Threading.Channels;

namespace Outboxer.Channels;

internal class OutboxChannel : IOutboxChannel
{
    /// <summary>
    /// This property holds the messages that will be sent to broker
    /// </summary>
    public Channel<Guid> EnqueuedItems { get; } = Channel.CreateUnbounded<Guid>();
    
    /// <summary>
    /// Adds a new message to channel
    /// </summary>
    /// <param name="entryId"></param>
    /// <returns></returns>
    public async Task Enqueue(Guid entryId)
    {
        await EnqueuedItems.Writer.WriteAsync(entryId);
    }

    /// <summary>
    /// Gets message from channel
    /// </summary>
    /// <returns></returns>
    public async Task<Guid> Peek()
    {
        return await EnqueuedItems.Reader.ReadAsync();
    }
}
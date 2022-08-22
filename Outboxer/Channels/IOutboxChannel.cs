using System.Threading.Channels;

namespace Outboxer.Channels;

public interface IOutboxChannel
{
    /// <summary>
    /// This property holds the messages that will be sent to broker
    /// </summary>
    Channel<Guid> EnqueuedItems { get; }
    
    /// <summary>
    /// Adds a new message to channel
    /// </summary>
    /// <param name="entryId"></param>
    /// <returns></returns>
    Task Enqueue(Guid entryId);
    /// <summary>
    /// Gets message from channel
    /// </summary>
    /// <returns></returns>
    Task<Guid> Peek();
}
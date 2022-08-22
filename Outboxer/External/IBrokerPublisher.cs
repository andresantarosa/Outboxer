using Outboxer.Models;

namespace Outboxer.External;

public interface IBrokerPublisher
{
    /// <summary>
    /// This is the method responsible for actually send your message to the queue
    /// Since the idea of this library is provide you only the outbox implementation
    /// you should implement this method by your own
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    Task<bool> Publish(Entry entry);
}
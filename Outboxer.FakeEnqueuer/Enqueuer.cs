using Outboxer.External;
using Outboxer.Models;

namespace Outboxer.FakeEnqueuer;

public class Enqueuer : IBrokerPublisher
{

    /// <summary>
    /// This property is here for test purpose only
    /// </summary>
    public static bool ReturnStatus { get; set; } = true;
    
    /// <summary>
    /// This is the method responsible for actually send your message to the queue
    /// Since the idea of this library is provide you only the outbox implementation
    /// you should implement this method by your own
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public async Task<bool> Publish(Entry entry)
    {
        return ReturnStatus;
    }
}
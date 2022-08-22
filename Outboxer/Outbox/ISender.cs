using Outboxer.Models;

namespace Outboxer.Outbox;

public interface IPublisher
{
    // <summary>
    /// Adds entry to Outbox repository and adds it to temporary area
    /// </summary>
    /// <param name="entry"></param>
    Task Publish(Entry entry);
    /// <summary>
    /// Adds entries to Outbox repository and adds them to temporary area
    /// </summary>
    /// <param name="entry"></param>
    Task Publish(List<Entry> entries);
    /// <summary>
    /// Set an entry as delivered at database
    /// </summary>
    /// <param name="entry"></param>
    void SetDelivered(Entry entry);
    /// <summary>
    /// Set an entry as failed at database
    /// </summary>
    /// <param name="entry"></param>
    void SetFailure(Entry entry);
}
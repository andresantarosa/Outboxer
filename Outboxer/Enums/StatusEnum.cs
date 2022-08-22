namespace Outboxer.Enums;

public enum StatusEnum
{
    /// <summary>
    /// Entry is still at the transactional area and is not at outbox table yet
    /// </summary>
    PENDING = 1,
    /// <summary>
    /// Entry is at outbox table and is ready to be sent
    /// </summary>
    ENQUEUED = 2,
    /// <summary>
    /// Entry is sent 
    /// </summary>
    DELIVERED = 3,
    /// <summary>
    /// Entry exceeded the configured amount of attempts to be delivered
    /// </summary>
    FAILED = 4
}
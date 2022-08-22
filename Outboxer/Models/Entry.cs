using System.Text.Json;
using Outboxer.Config;
using Outboxer.Enums;

namespace Outboxer.Models;

public class Entry
{
    protected Entry()
    {
    }

    /// <summary>
    /// Instantiates a new Entry.
    /// </summary>
    /// <param name="destination">Destination Queue (this field is meant to help you organizing things and will not have any direct influence on how Outboxer works)</param>
    /// <param name="content">The content of your message.</param>
    /// <param name="retries">How many times Outboxer should try to deliver your message. Default is 5</param>
    public Entry(string destination, object content, int retries = EntryDefaultConfig.DEFAULT_RETRIES)
    {
        Destination = destination;
        Content = JsonSerializer.Serialize(content);
        Retries = retries;
    }

    public Guid Id { get; } = Guid.NewGuid();
    public string Destination { get; }
    public string Content { get; private set; }
    public int Retries { get; } = EntryDefaultConfig.DEFAULT_RETRIES;
    public int RetriesCount { get; private set; }
    public DateTime DateCreated { get; private set; } = DateTime.Now;
    public DateTime LastInteraction { get; private set; } = DateTime.Now;
    public StatusEnum Status { get; private set; } = StatusEnum.PENDING;

    public void SetEnqueued() =>
        Status = StatusEnum.ENQUEUED;

    public void SetDelivered()
    {
        Status = StatusEnum.DELIVERED;
        LastInteraction = DateTime.Now;
    }

    public bool HasReachedRetryThreshold()
    {
        if (Retries <= 0)
            return false;

        return RetriesCount >= Retries;
    }

    public void IncreaseRetryCount()
    {
        if (!HasReachedRetryThreshold())
        {
            RetriesCount++;
            LastInteraction = DateTime.Now;
        }
        else
        {
            SetFailure();
        }
    }

    public void SetFailure()
    {
        Status = StatusEnum.FAILED;
        LastInteraction = DateTime.Now;
    }

    public void SetContent(object content) =>
        Content = JsonSerializer.Serialize(content);
}
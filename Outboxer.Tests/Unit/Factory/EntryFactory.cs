using Outboxer.Models;

namespace Outboxer.Tests.Unit.Factory;

public static class EntryFactory
{
    public static Entry CreateEntry(string destination, object content) =>
        new Entry(destination, content);

    public static object CreateEntryPayload(string name, string sureName) =>
        new
        {
            Name = name,
            SureName = sureName
        };
}
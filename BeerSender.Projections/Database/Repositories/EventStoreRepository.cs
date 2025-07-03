using System.Reflection;
using System.Text.Json;
using BeerSender.Domain;
using Dapper;

namespace BeerSender.Projections.Database.Repositories;

public class EventStoreRepository(EventStoreConnectionFactory dbFactory)
{
    public List<StoredEventWithVersion> GetEvents(
        IEnumerable<Type> types, byte[] lastVersion, int numberOfEvents)
    {
        var typeNames = types.Select(t => t.FullName).ToList();

        var query =
            """
            SELECT TOP (@NumberOfEvents)
                    [AggregateId], [SequenceNumber], [Timestamp],
                    [EventTypeName], [EventBody], [RowVersion]
            FROM    dbo.[Events]
            WHERE   [EventTypeName] in @TypeNames
                    AND [RowVersion] > @LastVersion
            ORDER BY 
                    [RowVersion]
            """;

        using var connection = dbFactory.CreateConnection();

        return connection.Query<DatabaseEvent>(
                query,
                new { TypeNames = typeNames, LastVersion = lastVersion, NumberOfEvents = numberOfEvents })
            .Select(e => e.ToStoredEventWithVersion())
            .ToList();
    }
}

public record DatabaseEvent
{
    public Guid AggregateId { get; set; }
    public int SequenceNumber { get; set; }
    public DateTime Timestamp { get; set; }
    public string? EventTypeName { get; set; }
    public string? EventBody { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public StoredEventWithVersion ToStoredEventWithVersion()
    {
        if (EventTypeName == null)
            throw new Exception("EventTypeName should not be null");
        if (EventBody == null)
            throw new Exception("EventBody should not be null");

        var eventType = _domainAssembly.GetType(EventTypeName);
        if (eventType == null)
            throw new Exception($"Type not Found: {EventTypeName}");

        var eventData = JsonSerializer.Deserialize(EventBody, eventType);
        if (eventData == null)
            throw new Exception($"Could not deserialize EventBody as {EventTypeName}");

        return new StoredEventWithVersion(
            AggregateId,
            SequenceNumber,
            Timestamp,
            eventData,
            RowVersion);
    }

    private static Assembly _domainAssembly = typeof(CommandRouter).Assembly;
}

public record StoredEventWithVersion(
    Guid AggregateId,
    int SequenceNumber,
    DateTime Timestamp,
    object EventData,
    byte[] RowVersion
);
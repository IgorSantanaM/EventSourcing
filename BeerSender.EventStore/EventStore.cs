using BeerSender.Domain;
using Dapper;

namespace BeerSender.EventStore;

public class EventStore(EventStoreConnectionFactory DbConnectionFactory) 
    : IEventStore
{
    public IEnumerable<StoredEvent> GetEvents(Guid aggregateId)
    {
        var query = """
            SELECT [AggregateId], [SequenceNumber], [Timestamp], [EventTypeName], [EventBody], [RowVersion]
            FROM dbo.[Events]
            WHERE [AggregateId = @AggregateId
            ORDER BY [SequenceNumber]
            """;

        using var connection = DbConnectionFactory.Create();

        return connection.Query<DatabaseEvent>(query, new
        {
            AggregateId = aggregateId,
        }).Select(e => e.ToStoredEvent());
    }

    private List<StoredEvent> _newEvents = new();
    public void AppendEvent(StoredEvent @event)
    {
        _newEvents.Add(@event);
    }

    public void SaveChanges()
    {
        var insertCommand = """
            INSERT into dbo.[Events]
                ([AggregateId], [SequenceNumber], [Timestamp], [EventTypeName], [EventBody])
                VALUES
                    (@AggregateId, @SequenceNumber, @Timestamp, @EventTypeName, @EventBody)
            """;
         
        using var connection = DbConnectionFactory.Create();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        connection.Execute(insertCommand, _newEvents.Select(DatabaseEvent.FromStoredEvent), 
            transaction);

        transaction.Commit();
        _newEvents.Clear();
    }
}
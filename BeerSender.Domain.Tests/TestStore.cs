using BeerSender.Domain;

namespace BeerSender.Domain.Tests;

/// <summary>
/// An in-memory Event Store for unit test purposes
/// </summary>
public class TestStore : IEventStore
{
    /// <summary>
    /// Add any events that have happened before to this collection.
    /// </summary>
    public List<StoredEvent> previousEvents = [];
    /// <summary>
    /// Use this collection to verify which events have been raised.
    /// </summary>
    public List<StoredEvent> newEvents = [];

    /// <summary>
    /// Gets the events from "previousEvents" for the Aggregate ID.
    /// </summary>
    public IEnumerable<StoredEvent> GetEvents(Guid aggregateId)
    {
        return previousEvents
            .Where(e => e.AggreagateId== aggregateId)
            .ToList();
    }

    /// <summary>
    /// Appends new events to the "newEvents' collection.
    /// </summary>
    public void AppendEvent(StoredEvent @event)
    {
        newEvents.Add(@event);
    }

    /// <summary>
    /// Not used in command handle tests
    /// </summary>
    public void SaveChanges()
    {
        throw new NotImplementedException();
    }
}
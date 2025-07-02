using FluentAssertions;

namespace BeerSender.Domain.Tests;

/// <summary>
/// Test base class for CommandHandler tests.
/// </summary>
/// <typeparam name="TCommand">The command type for the handler.</typeparam>
public abstract class CommandHandlerTest<TCommand>
{
    /// <summary>
    /// If no explicit aggregateId is provided, this one will be used behind the scenes.
    /// </summary>
    protected readonly Guid _aggregateId = Guid.NewGuid();

    /// <summary>
    /// The command handler, to be provided in the Test class.
    /// This to account for additional injections
    /// </summary>
    protected abstract CommandHandler<TCommand> Handler { get; }

    /// <summary>
    /// A fake, in-memory event store.
    /// </summary>
    protected TestStore eventStore = new();

    /// <summary>
    /// Sets a list of previous events for the default aggregate ID.
    /// </summary>
    protected void Given(params object[] events)
    {
        Given(_aggregateId, events);
    }

    /// <summary>
    /// Sets a list of previous events for a specified aggregate ID.
    /// </summary>
    protected void Given(Guid aggregateId, params object[] events)
    {
        eventStore.previousEvents.AddRange(events
            .Select((e,i) => new StoredEvent(aggregateId, i, DateTime.Now, e)));
    }

    /// <summary>
    /// Triggers the handling of a command against the configured events.
    /// </summary>
    protected void When(TCommand command)
    {
        Handler.Handle(command);
    }

    /// <summary>
    /// Asserts that the expected events have been appended to the event store
    /// for the default aggregate ID.
    /// </summary>
    protected void Then(params object[] expectedEvents)
    {
        Then(_aggregateId, expectedEvents);
    }

    /// <summary>
    /// Asserts that the expected events have been appended to the event store
    /// for a specific aggregate ID.
    /// </summary>
    protected void Then(Guid aggregateId, params object[] expectedEvents)
    {
        var actualEvents = eventStore.newEvents
            .Where(e => e.AggreagateId == aggregateId)
            .OrderBy(e => e.SequenceNumber)
            .Select(e => e.EventData)
            .ToArray();

        actualEvents.Length.Should().Be(expectedEvents.Length);

        for (var i = 0; i < actualEvents.Length; i++)
        {
            actualEvents[i].Should().BeOfType(expectedEvents[i].GetType());
            try
            {
                actualEvents[i].Should().BeEquivalentTo(expectedEvents[i]);
            }
            catch (InvalidOperationException e)
            {
                // Empty event with matching type is OK. This means that the event class
                // has no properties. If the types match in this situation, the correct
                // event has been appended. So we should ignore this exception.
                if (!e.Message.StartsWith("No members were found for comparison."))
                    throw;
            }
        }
    }
}
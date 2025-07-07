using FluentAssertions;
using Marten;
using Marten.Linq.Parsing.Operators;

namespace BeerSender.Domain.Tests;

[Collection("Marten collection")]
public abstract class CommandHandlerTest<TCommand>
{
    private readonly Dictionary<Guid, long> _streamVersion = new();

    MartenFixture _fixture;

    protected CommandHandlerTest(MartenFixture fixture)
    {
        _fixture = fixture;
        Store = fixture.Store;
    }

    protected readonly Guid _aggregateId = Guid.NewGuid();

    protected abstract ICommandHandler<TCommand> Handler { get; }

    protected IDocumentStore Store { get; private set; }

    protected async Task Given(params object[] events)
    {
        await Given(_aggregateId, events);
    }

  
    protected async Task Given<TAggregate>(params object[] events) where TAggregate : class
    {
        await Given<TAggregate>(_aggregateId, events);
    }

    protected async Task Given<TAggregate>(Guid aggregateId, params object[] events) where TAggregate : class
    {
        if(events.IsEmpty()) return;

        await using var session = Store.LightweightSession();
        var stream = session.Events.StartStream<TAggregate>(aggregateId, events);

        _streamVersion[aggregateId] = stream.Version + stream.Events.Count;

        await session.SaveChangesAsync();
    }


    protected async Task When(TCommand command)
    {
       await Handler.Handle(command);
    }

    protected async Task Then(params object[] expectedEvents)
    {
       await Then(_aggregateId, expectedEvents);
    }

   
    protected async Task Then(Guid aggregateId, params object[] expectedEvents)
    {

        await using var session = Store.LightweightSession();
        var versin = _streamVersion.ContainsKey(aggregateId) ? _streamVersion[aggregateId] + 1 : 0L;

        var storedEvents = await session.Events.FetchStreamAsync(aggregateId, versin);
        
        var actualEvents = storedEvents
            .OrderBy(e => e.Version)
            .Select(e => e.Data)
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
                if (!e.Message.StartsWith("No members were found for comparison."))
                    throw;
            }
        }
    }
}
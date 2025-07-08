using Marten;

namespace BeerSender.Domain.Tests
{
    [Collection("Marten collection")]
    public abstract class MartenTest(MartenFixture fixture)
    {
        protected IDocumentStore Store { get; private set; } = fixture.Store;
    }
}

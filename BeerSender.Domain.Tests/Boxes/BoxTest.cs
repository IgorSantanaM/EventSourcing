using BeerSender.Domain.Boxes;

namespace BeerSender.Domain.Tests.Boxes
{
    public abstract class BoxTest<TCommand>(MartenFixture fixture) : CommandHandlerTest<TCommand>(fixture)
    {
        protected Guid Box_Id => _aggregateId;

        // Events
        protected BoxCreated Box_created_with_capacity(int capacity)
        {
            return new BoxCreated(new BoxCapacity(capacity));
        }

        protected BeerBottleAdded Beer_bottle_added(BeerBottle bottle)
        {
            return new BeerBottleAdded(bottle);
        } 

        //Test Data
        protected BeerBottle carte_blanche = new(
                    "Wolf ",
                    "Carte Blanche",
                    8.5,
                    BeerType.Triple);
    }
}
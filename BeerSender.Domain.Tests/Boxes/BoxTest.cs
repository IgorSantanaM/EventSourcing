using BeerSender.Domain.Boxes;

namespace BeerSender.Domain.Tests.Boxes
{
    public abstract class BoxTest<TCommand>(MartenFixture fixture) : CommandHandlerTest<TCommand>(fixture)
        where TCommand : class, ICommand
    {
        protected Guid Box_Id => _aggregateId;

        // Events
        protected BoxCreatedWithContainerType Box_created_with_capacity(int capacity)
        {
            return new BoxCreatedWithContainerType(new BoxCapacity(capacity), string.Empty, ContainerType.Bottle);
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
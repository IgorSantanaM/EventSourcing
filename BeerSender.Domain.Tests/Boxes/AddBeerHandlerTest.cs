using BeerSender.Domain.Boxes;
using BeerSender.Domain.Boxes.Commands;

namespace BeerSender.Domain.Tests.Boxes
{

    public abstract class BoxTest<TCommand> : CommandHandlerTest<TCommand>
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

    public class AddBeerHandlerTest : BoxTest<AddBeerBottle>
    {
        protected override CommandHandler<AddBeerBottle> Handler => new AddBeerBottleHandler(eventStore);

        [Fact]
        public void IfBoxIsEmpty_ThenBottleShouldBeAdded()
        {

            Given(
                Box_created_with_capacity(6)
                );

            When(
                Add_beer_bottle(carte_blanche)
                );

            Then(
                Beer_bottle_added(carte_blanche)
                );
        }

        // Commands 
        private AddBeerBottle Add_beer_bottle(BeerBottle beerBottle)
        {
            return new AddBeerBottle(Box_Id,
                    beerBottle);
        }
    }
}

using BeerSender.Domain.Boxes;
using BeerSender.Domain.Boxes.Commands;

namespace BeerSender.Domain.Tests.Boxes
{

    public class AddBeerHandlerTest(MartenFixture fixture) : BoxTest<AddBeerBottle>(fixture)
    {
        protected override ICommandHandler<AddBeerBottle> Handler => new AddBeerBottleHandler();

        [Fact]
        public async Task IfBoxIsEmpty_ThenBottleShouldBeAdded()
        {
            await Given<Box>(
               Box_created_with_capacity(6)
               );

            await When(
                Add_beer_bottle(carte_blanche)
                );

            await Then(
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
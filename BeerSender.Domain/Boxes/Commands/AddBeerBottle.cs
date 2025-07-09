using Marten;

namespace BeerSender.Domain.Boxes.Commands;

public record AddBeerBottle 
(
    Guid BoxId,
    BeerBottle BeerBottle
) : ICommand;

public class AddBeerBottleHandler()
    : ICommandHandler<AddBeerBottle>
{
    public async Task Handle(IDocumentSession session, AddBeerBottle command)
    {
        var stream = await session.Events.FetchForWriting<Box>(command.BoxId);

        var box = stream.Aggregate;

        if (box!.IsFull)
        {
            stream.AppendOne(new FailedToAddBeerBottle(FailedToAddBeerBottle.FailReason.BoxWasFull));
        }
        else
        {
            stream.AppendOne(new BeerBottleAdded(command.BeerBottle));
        }
    }
}
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
        var box = await session.Events.FetchLatest<Box>(command.BoxId);

        if (box!.IsFull)
        {
            session.Events.Append(command.BoxId, new FailedToAddBeerBottle(FailedToAddBeerBottle.FailReason.BoxWasFull));
        }
        else
        {
            session.Events.Append(command.BoxId, new BeerBottleAdded(command.BeerBottle));
        }
    }
}
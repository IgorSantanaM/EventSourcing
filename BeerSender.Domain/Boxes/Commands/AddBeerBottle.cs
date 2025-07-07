using Marten;

namespace BeerSender.Domain.Boxes.Commands;

public record AddBeerBottle
(
    Guid BoxId,
    BeerBottle BeerBottle
);

public class AddBeerBottleHandler(IDocumentStore store)
    : ICommandHandler<AddBeerBottle>
{
    public async Task Handle(AddBeerBottle command)
    {
        await using var session = store.IdentitySession();

        var box = await session.Events.AggregateStreamAsync<Box>(command.BoxId);

        if (box!.IsFull)
        {
            session.Events.Append(command.BoxId, new FailedToAddBeerBottle(FailedToAddBeerBottle.FailReason.BoxWasFull));
        }
        else
        {
            session.Events.Append(command.BoxId, new BeerBottleAdded(command.BeerBottle));
        }

        await session.SaveChangesAsync();
    }
}
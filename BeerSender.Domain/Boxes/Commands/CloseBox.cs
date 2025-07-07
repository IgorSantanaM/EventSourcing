using Marten;
using System.Runtime.InteropServices;

namespace BeerSender.Domain.Boxes.Commands;

public record CloseBox
(
    Guid BoxId
);

public class CloseBoxHandler(IDocumentStore store)
    : ICommandHandler<CloseBox>
{
    public async Task Handle(CloseBox command)
    {
        await using var session = store.IdentitySession();
        var box = await session.Events.AggregateStreamAsync<Box>(command.BoxId);

        if (box!.BeerBottles.Any())
        {
            session.Events.Append(command.BoxId, new BoxClosed());
        }
        else
        {
            session.Events.Append(command.BoxId, new FailedToCloseBox(FailedToCloseBox.FailReason.BoxWasEmpty));
        }

        await session.SaveChangesAsync();
    }
}
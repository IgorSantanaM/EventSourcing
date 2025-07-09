using Marten;
using System.Runtime.InteropServices;

namespace BeerSender.Domain.Boxes.Commands;

public record CloseBox
(
    Guid BoxId
) : ICommand;

public class CloseBoxHandler(IDocumentStore store)
    : ICommandHandler<CloseBox>
{
    public async Task Handle(IDocumentSession session, CloseBox command)
    {
        var stream = await session.Events.FetchForWriting<Box>(command.BoxId);
        var box = stream.Aggregate;

        if (box!.BeerBottles.Any())
        {
            stream.AppendOne(new BoxClosed());
        }
        else
        {
            stream.AppendOne(new FailedToCloseBox(FailedToCloseBox.FailReason.BoxWasEmpty));
        }
    }
}
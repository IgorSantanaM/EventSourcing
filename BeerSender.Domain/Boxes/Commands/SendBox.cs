using Marten;

namespace BeerSender.Domain.Boxes.Commands;

public record SendBox(
    Guid BoxId
) : ICommand;

public class SendBoxHandler(IDocumentStore store)
    : ICommandHandler<SendBox>
{
    public async Task Handle(IDocumentSession session, SendBox command)
    {
        var box = await session.Events.AggregateStreamAsync<Box>(command.BoxId);

        var success = true;
        
        if (!box!.IsClosed)
        {
            session.Events.Append(command.BoxId, new FailedToSendBox(FailedToSendBox.FailReason.BoxWasNotClosed));
            success = false;
        }

        if (box.ShippingLabel is null)
        {
            session.Events.Append(command.BoxId, new FailedToSendBox(FailedToSendBox.FailReason.BoxHadNoLabel));
            success = false;
        }
        
        if(success)
        {
            session.Events.Append(command.BoxId, new BoxSent());
        }
    }
}
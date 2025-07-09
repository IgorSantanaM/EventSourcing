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

        var stream = await session.Events.FetchForWriting<Box>(command.BoxId);
        var box = stream.Aggregate;

        var success = true;
        
        if (!box!.IsClosed)
        {
            stream.AppendOne(new FailedToSendBox(FailedToSendBox.FailReason.BoxWasNotClosed));
            success = false;
        }

        if (box.ShippingLabel is null)
        {
            stream.AppendOne(new FailedToSendBox(FailedToSendBox.FailReason.BoxHadNoLabel));
            success = false;
        }
        
        if(success)
        {
            stream.AppendOne(new BoxSent());
        }
    }
}
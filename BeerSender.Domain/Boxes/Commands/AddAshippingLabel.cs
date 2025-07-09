using Marten;

namespace BeerSender.Domain.Boxes.Commands;

public record AddShippingLabel(
    Guid BoxId,
    ShippingLabel Label) : ICommand;

public class AddShippingLabelHandler(IDocumentStore store)
    : ICommandHandler<AddShippingLabel>
{
    public async Task Handle(IDocumentSession session, AddShippingLabel command)
    {
        var stream = await session.Events.FetchForWriting<Box>(command.BoxId);

        if (command.Label.IsValid())
        {
            stream.AppendOne(new ShippingLabelAdded(command.Label));
        }
        else
        {
            stream.AppendOne(new FailedToAddShippingLabel(FailedToAddShippingLabel.FailReason.TrackingCodeInvalid));
        }
    }
}










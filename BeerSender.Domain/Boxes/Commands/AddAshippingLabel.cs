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
        var box = session.Events.AggregateStreamAsync<Box>(command.BoxId);

        if (command.Label.IsValid())
        {
            session.Events.Append(command.BoxId, new ShippingLabelAdded(command.Label));
        }
        else
        {
            session.Events.Append(command.BoxId, new FailedToAddShippingLabel(
                FailedToAddShippingLabel.FailReason.TrackingCodeInvalid));
        }
    }
}










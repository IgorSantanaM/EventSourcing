using Marten;

namespace BeerSender.Domain.Boxes.Commands;

public record AddShippingLabel(
    Guid BoxId,
    ShippingLabel Label);

public class AddShippingLabelHandler(IDocumentStore store)
    : ICommandHandler<AddShippingLabel>
{
    public async Task Handle(AddShippingLabel command)
    {
        await using var session = store.IdentitySession();
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

        await session.SaveChangesAsync();
    }
}










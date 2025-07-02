using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerSender.Domain.Boxes.Commands;
 public record AddShippingLabel(
        Guid BoxId,
        ShippingLabel Label);

public class AddShippingLabelHandler(IEventStore eventStore)
    : CommandHandler<AddShippingLabel>(eventStore)
{
    public override void Handle(AddShippingLabel command)
    {
        var boxStream = GetStream<Box>(command.BoxId);
        var box = boxStream.GetEntity();

        if (command.Label.IsValid())
        {
            boxStream.Append(new ShippingLabelAdded(command.Label));
        }
        else
        {
            boxStream.Append(new ShippingLabelFailedToAdd(ShippingLabelFailedToAdd.FailReason.TrackingCodeInvalid));
        }        
    }
}

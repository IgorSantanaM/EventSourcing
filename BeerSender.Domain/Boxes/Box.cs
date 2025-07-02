using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerSender.Domain.Boxes
{
    public class Box : AggregateRoot
    {
        public void Apply(BoxCreated @event)
        {
            Capacity = @event.Capacity;
        }
        public void Apply(ShippingLabelAdded @event)
        {
            ShippingLabel = @event.Label;
        }
        public BoxCapacity? Capacity{ get; private set; }
        public ShippingLabel? ShippingLabel { get; private set; }
    }

    public record BoxCreated(
    BoxCapacity Capacity
    );

    public record ShippingLabelAdded(
        ShippingLabel Label);

    public record ShippingLabelFailedToAdd(ShippingLabelFailedToAdd.FailReason Reason)
    {
        public enum FailReason
        {
            TrackingCodeInvalid,
        }
    };

    public enum Carrier
    {
        UPS,
        FedEx,
        BPost
    }

    public record ShippingLabel(Carrier Carrier, string TrackingCode)
    {
        public bool IsValid()
        {
            return Carrier switch
            { 
                Carrier.UPS => TrackingCode.StartsWith("ABC"),
                Carrier.FedEx => TrackingCode.StartsWith("DEF"),
                Carrier.BPost => TrackingCode.StartsWith("GHI"),
                _ => throw new ArgumentOutOfRangeException(nameof(Carrier), Carrier, null),
            };
        }
    }

    public record BoxCapacity(int NumberOfSpots)
    {
        public static BoxCapacity Create(int desiredNumberOfSpots)
        {
            return desiredNumberOfSpots switch
            {
                <= 6 => new BoxCapacity(6),
                <= 12 => new BoxCapacity(12),
                _ => new BoxCapacity(24),
            };
        }
    }
}

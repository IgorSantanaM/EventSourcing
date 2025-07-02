namespace BeerSender.Domain.Boxes
{
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
}

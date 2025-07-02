namespace BeerSender.Domain.Boxes
{
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

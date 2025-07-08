using BeerSender.Domain.Boxes;
using Marten.Events.Projections;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BeerSender.Domain.Projections
{
    public class OpenBox
    {
        public Guid BoxId { get; set; }
        public int Capacity { get; set; }
        public int NumberOfBottles { get; set; }
    }

    public class OpenBoxProjection : SingleStreamProjection<OpenBox, Guid>
    {
        public OpenBoxProjection()
        {
            DeleteEvent<BoxClosed>();    
        }

        public static OpenBox Create(BoxCreatedWithContainerType started)
        {
            return new OpenBox
            {
                Capacity = started.BoxType.NumberOfSpots
            };
        }

        public static void Apply(BeerBottleAdded _, OpenBox box) =>
            box.NumberOfBottles++;


    }
}

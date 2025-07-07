using BeerSender.Domain.Boxes;
using Marten.Events.Projections;
using JasperFx.Events;

namespace BeerSender.Domain.Projections
{
    public class OpenBox
    {
        public Guid BoxId { get; set; }
        public int Capacity { get; set; }
        public int NumberOfBottles { get; set; }
    }

    public class OpenBoxProjection : EventProjection
    {
        public OpenBoxProjection()
        {
            Project<IEvent<BoxCreated>>((evt, operations) =>
            {
                operations.Store(new OpenBox
                {
                    BoxId = evt.StreamId,
                    Capacity = evt.Data.Capacity.NumberOfSpots
                });
            });

            Project<IEvent<BoxClosed>>((evt, operations) =>
            {
                operations.Delete<OpenBox>(evt.StreamId);
            });

            ProjectAsync<IEvent<BeerBottleAdded>>(async (evt, operations, c) =>
            {
                var openBox = await operations.LoadAsync<OpenBox>(evt.StreamId);

                if (openBox is null) return;
                openBox.NumberOfBottles++;
                operations.Store(openBox);
            });
        }
    }
}

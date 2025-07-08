using BeerSender.Domain.Boxes;
using JasperFx.Events;
using Marten.Events.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerSender.Domain.Projections
{
    public class BottleInBoxes
    {
        public required string BottleId { get; set; }
        public required BeerBottle Bottle { get; set; }
        public List<Guid> BoxIds { get; set; } = new();
    }

    public class BottleiInBoxesProjection : MultiStreamProjection<BottleInBoxes, string>
    {
        public BottleiInBoxesProjection()
        {
            Identity<BeerBottleAdded>(x => x.Bottle.BottleId);
        }

        public static BottleInBoxes Create(IEvent<BeerBottleAdded> started)
        {
            return new BottleInBoxes
            {
                BottleId = started.Data.Bottle.BottleId,
                Bottle = started.Data.Bottle,
                BoxIds = [started.StreamId]
            };
        }

        public static void Apply(IEvent<BeerBottleAdded> evt, BottleInBoxes bottle)
        {
            bottle.BoxIds.Add(evt.StreamId);
        }
    }
}

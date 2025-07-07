using BeerSender.Domain.Boxes;
using JasperFx.Events;
using Marten.Events.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BeerSender.Domain.Projections
{
    public class UnsentBox
    {
        public Guid BoxId { get; set; }
        public string? Status { get; set; }
    }

    public class UnsentBoxProjection : EventProjection
    {
        public UnsentBoxProjection()
        {
            Project<IEvent<BoxCreated>>((evt, operations) =>
            {
                operations.Store(new UnsentBox
                {
                    BoxId = evt.StreamId
                });
            });

            Project<IEvent<BoxSent>>((evt, operations) =>
            {
                operations.Delete<UnsentBox>(evt.StreamId);
            });

            ProjectAsync<IEvent<BoxClosed>>(async (evt, operations, c) =>
            {
                var unsentBox = await operations.LoadAsync<UnsentBox>(evt.StreamId);

                if (unsentBox is null) return;

                unsentBox.Status = "Ready to send!";

                operations.Store(unsentBox);
            });
        }
    }
}

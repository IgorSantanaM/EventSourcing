using BeerSender.Domain;
using Microsoft.AspNetCore.SignalR;

namespace BeerSender.Web.EventPublishing
{
    public class NotificationService(IHubContext<EventHub> hubContext) : INotificationService
    {
        public void PublishEvent(Guid aggregateId, object @event)
        {
            hubContext.Clients.Group(aggregateId.ToString())
                .SendAsync("PublishEvent", aggregateId, @event, @event.GetType().Name);
        }
    }
}

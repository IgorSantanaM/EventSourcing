namespace BeerSender.Domain
{
    public interface INotificationService
    {
        void PublishEvent(Guid aggregateId, object @event);
    }
}

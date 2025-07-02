namespace BeerSender.Domain
{
    public interface IEventStore
    {
        IEnumerable<StoredEvent> GetEvents(Guid aggregateId);
        void AppendEvent(StoredEvent @event);
        void SaveChanges(); // UoW pattern applicable
    }
}

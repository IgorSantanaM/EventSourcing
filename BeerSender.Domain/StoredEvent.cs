using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerSender.Domain
{
    public record StoredEvent(
        Guid AggreagateId, 
        int SequenceNumber,
        DateTime TimeStamp,
        object EventData);


    public interface IEventStore
    {
        IEnumerable<StoredEvent> GetEvents(Guid aggregateId);
        void AppendEvent(StoredEvent @event);
        void SaveChanges(); // UoW pattern applicable
    }

    public class EventStream<TEntity>(IEventStore eventStore, Guid aggreagateId) where TEntity : AggregateRoot, new()
    {
        private int _lastSequenceNumber;
        public TEntity GetEntity()
        {
            var events = eventStore.GetEvents(aggreagateId);

            TEntity entity = new();

            foreach(var @event in events)
            {
                entity.Apply((dynamic)@event.EventData);
                _lastSequenceNumber = @event.SequenceNumber;
            }

            return entity;
        }
        public void Append(object @event)
        {
            _lastSequenceNumber++;
            StoredEvent storedEvent = new(
                aggreagateId, _lastSequenceNumber, DateTime.UtcNow,@event);

            eventStore.AppendEvent(storedEvent);
        }
    }
}

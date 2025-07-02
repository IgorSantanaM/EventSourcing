namespace BeerSender.Domain
{
    public abstract class CommandHandler<TCommand>(IEventStore eventStore)
    {
        protected EventStream<TEntity> GetStream<TEntity>(Guid aggreagateId) where TEntity : AggregateRoot, new()
        {
            return new EventStream<TEntity>(eventStore, aggreagateId);
        }
        public abstract void Handle(TCommand command);
    }
}

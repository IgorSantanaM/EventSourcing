namespace BeerSender.Domain
{
    public interface ICommandHandler<in TCommand>
    {
        public Task Handle(TCommand command);
    }
}

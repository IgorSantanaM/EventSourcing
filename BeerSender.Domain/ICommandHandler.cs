using Marten;

namespace BeerSender.Domain
{
    public interface ICommandHandler<in TCommand> where TCommand : class, ICommand
    {
        public Task Handle(IDocumentSession session, TCommand command);
    }
    public interface ICommand;
}

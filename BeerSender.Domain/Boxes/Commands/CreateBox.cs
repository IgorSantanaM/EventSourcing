using Marten;

namespace BeerSender.Domain.Boxes.Commands;

public record CreateBox(
    Guid BoxId,
    int DesiredNumberOfSpots,
    string FriendlyName,
    ContainerType ContainerType
) : ICommand;

public class CreateBoxHandler(IDocumentStore store)
    : ICommandHandler<CreateBox>
{
    public Task Handle(IDocumentSession session, CreateBox command)
    {
        var capacity = BoxCapacity.Create(command.DesiredNumberOfSpots);

        session.Events.StartStream<Box>(command.BoxId, new BoxCreatedWithContainerType(capacity, command.FriendlyName, command.ContainerType));

        return Task.CompletedTask;
    }
}
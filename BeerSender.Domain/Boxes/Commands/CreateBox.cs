using Marten;

namespace BeerSender.Domain.Boxes.Commands;

public record CreateBox(
    Guid BoxId,
    int DesiredNumberOfSpots,
    string FriendlyName,
    ContainerType ContainerType
);

public class CreateBoxHandler(IDocumentStore store)
    : ICommandHandler<CreateBox>
{
    public async Task Handle(CreateBox command)
    {
        await using var session = store.IdentitySession();

        var capacity = BoxCapacity.Create(command.DesiredNumberOfSpots);

        session.Events.StartStream<Box>(command.BoxId, new BoxCreated(capacity, command.FriendlyName, command.ContainerType));

        await session.SaveChangesAsync();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerSender.Domain.Boxes.Commands
{
    public record CreateBox(
    Guid BoxId,
    int DesiredNumberOfSpots
    );

    public class CreateBoxHandler(IEventStore eventStore) : CommandHandler<CreateBox>(eventStore)
    {
        public override void Handle(CreateBox command)
        {
            var boxStream = GetStream<Box>(command.BoxId);
            var box = boxStream.GetEntity();

            var boxCapacity = new BoxCapacity(command.DesiredNumberOfSpots);

            boxStream.Append(new BoxCreated(boxCapacity));
        }
    }
}
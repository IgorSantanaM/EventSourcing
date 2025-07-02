namespace BeerSender.Domain.Boxes.Commands;

public record CloseBox
(
    Guid BoxId
);

public class CloseBoxHandler(IEventStore eventStore)
    : CommandHandler<CloseBox>(eventStore)
{
    public override void Handle(CloseBox command)
    {
        var boxStream = GetStream<Box>(command.BoxId);
        var box = boxStream.GetEntity();


        if (box.BeerBottles.Any())
        {
            boxStream.Append(new BoxClosed());
        }
        else
        {
            boxStream.Append(new FailedToCloseBox(FailedToCloseBox.FailReason.BoxWasEmpty));
        }
    }
}
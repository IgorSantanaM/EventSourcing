namespace BeerSender.Domain
{
    public record StoredEvent(
        Guid AggreagateId,
        int SequenceNumber,
        DateTime TimeStamp,
        object EventData);
}

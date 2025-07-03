using BeerSender.Projections.Database.Repositories;

namespace BeerSender.Projections;

public interface IProjection
{
    List<Type> RelevantEventTypes { get; }
    int BatchSize { get; }
    int WaitTime { get; }
    void Project(IEnumerable<StoredEventWithVersion> events);
}
using BeerSender.Projections.Database;
using BeerSender.Projections.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.Tracing;

namespace BeerSender.Projections
{
    public class ProjectionService<TProjection>(
        IServiceProvider serviceProvider,
        EventStoreRepository eventsRepo) : BackgroundService
        where TProjection : class, IProjection
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var checkpoint = GetCheckpoint();
            
            while(!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();

                var connection = scope.ServiceProvider.GetRequiredService<ReadStoreConnection>();
                using var transaction = connection.GetTransaction();

                var projection = scope.ServiceProvider
                    .GetRequiredService<TProjection>();

                var events = eventsRepo.GetEvents(
                    projection.RelevantEventTypes,
                    checkpoint,
                    projection.BatchSize
                );

                if (!events.Any()) 
                { 
                    await Task.Delay(projection.WaitTime);
                }
                else
                {
                    projection.Project(events);

                    checkpoint = events.Last().RowVersion;
                    var checkpointRepo = scope.ServiceProvider.GetRequiredService<CheckpointRepository>();

                    checkpointRepo.SetCheckpoint(typeof(TProjection).Name, checkpoint);
                }

                    transaction.Commit();
            }
        }

        private byte[] GetCheckpoint()
        {
            using var scope = serviceProvider.CreateScope();
            var checkpointService = scope.ServiceProvider
                .GetRequiredService<CheckpointRepository>();

            var checkpoint = checkpointService.GetCheckpoint(typeof(TProjection).Name);
            return checkpoint;
        }
    }
}

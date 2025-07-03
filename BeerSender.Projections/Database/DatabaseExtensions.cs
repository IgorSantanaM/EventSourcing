using Microsoft.Extensions.DependencyInjection;
using BeerSender.Projections.Database.Repositories;

namespace BeerSender.Projections.Database;

static class DatabaseExtensions
{
    public static void RegisterDataConnections(this IServiceCollection services)
    {
        services.AddSingleton<EventStoreConnectionFactory>();
        services.AddTransient<EventStoreRepository>();

        services.AddScoped<ReadStoreConnection>();

        services.AddScoped<CheckpointRepository>();
        services.AddScoped<OpenBoxRepository>();

    }
}
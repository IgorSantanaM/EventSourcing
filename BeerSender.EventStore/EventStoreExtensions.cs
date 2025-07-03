using BeerSender.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerSender.EventStore
{
    public static class EventStoreExtensions
    {
        public static void RegisterEventStore(this IServiceCollection services)
        {
            services.AddSingleton<EventStoreConnectionFactory>();
            services.AddScoped<IEventStore, EventStore>();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerSender.Projections.Projections
{
    public static class ProjectionExtensions
    {
        public static void RegisterProjections(this IServiceCollection services)
        {
            services.AddTransient<OpenBoxProjection>();
            services.AddHostedService<ProjectionService<OpenBoxProjection>>();
        }
    }
}

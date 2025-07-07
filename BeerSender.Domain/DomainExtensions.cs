using BeerSender.Domain.Boxes.Commands;
using BeerSender.Domain.Projections;
using JasperFx.Events.Projections;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerSender.Domain
{
    public static class DomainExtensions
    {
        public static void RegisterDomain(this IServiceCollection services)
        {
            services.AddScoped<CommandRouter>();

            services.AddTransient<ICommandHandler<CreateBox>, CreateBoxHandler>();
            services.AddTransient<ICommandHandler<AddShippingLabel>, AddShippingLabelHandler>();
            services.AddTransient<ICommandHandler<AddBeerBottle>, AddBeerBottleHandler>();
            services.AddTransient<ICommandHandler<CloseBox>, CloseBoxHandler>();
            services.AddTransient<ICommandHandler<SendBox>, SendBoxHandler>();
        }
        
        public static void ApplyDomainConfig(this StoreOptions options)
        {
            options.UseSystemTextJsonForSerialization();

            options.Schema.For<UnsentBox>().Identity(u => u.BoxId);
            options.Schema.For<OpenBox>().Identity(u => u.BoxId);
        }

        public static void AddProjections(this StoreOptions options)
        {
            options.Projections.Add<UnsentBoxProjection>(ProjectionLifecycle.Async);
            options.Projections.Add<OpenBoxProjection>(ProjectionLifecycle.Async);
        }
    }
}

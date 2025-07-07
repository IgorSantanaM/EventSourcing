using BeerSender.Domain.Boxes.Commands;
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
    }
}

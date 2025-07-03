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

            services.AddTransient<CommandHandler<CreateBox>, CreateBoxHandler>();
            services.AddTransient<CommandHandler<AddShippingLabel>, AddShippingLabelHandler>();
            services.AddTransient<CommandHandler<AddBeerBottle>, AddBeerBottleHandler>();
            services.AddTransient<CommandHandler<CloseBox>, CloseBoxHandler>();
            services.AddTransient<CommandHandler<SendBox>, SendBoxHandler>();
        }
    }
}

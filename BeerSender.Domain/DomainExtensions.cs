﻿using BeerSender.Domain.Boxes;
using BeerSender.Domain.Boxes.Commands;
using BeerSender.Domain.JsonConfiguration;
using BeerSender.Domain.Projections;
using JasperFx.Events.Projections;
using Marten;
using Marten.Services.Json.Transformations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
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
            options.UseSystemTextJsonForSerialization(configure: opt =>
            {
                opt.TypeInfoResolver = new CommandTypeResolver();
            });

            options.Events.MetadataConfig.CorrelationIdEnabled = true;
            options.Events.MetadataConfig.CausationIdEnabled = true;
            options.Events.MetadataConfig.HeadersEnabled = true;

            options.Schema.For<UnsentBox>().Identity(u => u.BoxId);
            options.Schema.For<OpenBox>().Identity(u => u.BoxId);
            options.Schema.For<BottleInBoxes>().Identity(u => u.BottleId);
            options.Schema.For<LoggedCommand>().Identity(u => u.CommandId);

            options.Events.Upcast<BoxCreatedUpcaster>();
        }

        public static void AddProjections(this StoreOptions options)
        {
            options.Projections.Add<UnsentBoxProjection>(ProjectionLifecycle.Async);
            options.Projections.Add<OpenBoxProjection>(ProjectionLifecycle.Async);
            options.Projections.Add<BottleiInBoxesProjection>(ProjectionLifecycle.Async);

            options.Projections.Snapshot<Box>(Marten.Events.Projections.SnapshotLifecycle.Async);
        }
    }

    public class BoxCreatedUpcaster
         : EventUpcaster<BoxCreated, BoxCreatedWithContainerType>
    {
        protected override BoxCreatedWithContainerType Upcast(BoxCreated oldEvent)
        {
            return new BoxCreatedWithContainerType(oldEvent.Capacity, string.Empty, ContainerType.Bottle);
        }
    }
}

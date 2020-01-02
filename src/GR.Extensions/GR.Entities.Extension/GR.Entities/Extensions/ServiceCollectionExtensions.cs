﻿using Castle.MicroKernel.Registration;
using GR.Core.Helpers;
using GR.Entities.Abstractions.Events;
using GR.Entities.Data;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Entities.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register entity event job
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterEntityBuilderJob(this IServiceCollection services)
        {
            IoC.Container.Register(Component.For<EntitySynchronizer>());

            EntityEvents.Entities.OnEntityAddNewField += (sender, args) =>
            {
               
            };

            EntityEvents.Entities.OnEntityCreated += (sender, args) =>
            {
                
            };

            EntityEvents.Entities.OnEntityDeleteField += (sender, args) =>
            {
                
            };

            EntityEvents.Entities.OnEntityDeleted += (sender, args) =>
            {
               
            };

            EntityEvents.Entities.OnEntityUpdateField += (sender, args) =>
            {
                
            };

            EntityEvents.Entities.OnEntityUpdated += (sender, args) =>
            {
               
            };

            return services;
        }
    }
}

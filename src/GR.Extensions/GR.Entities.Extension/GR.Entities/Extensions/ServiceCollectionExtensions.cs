using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using GR.Core.Helpers;
using GR.Entities.Abstractions.Events;
using GR.Entities.Controls.Builders;
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

            //TODO: On entity change change only the updated section not remove entire entity 
            EntityEvents.Entities.OnEntityAddNewField += (sender, args) =>
            {
                ViewModelBuilderFactory.ResetBuildEntity(args?.EntityName);
            };

            EntityEvents.Entities.OnEntityCreated += (sender, args) =>
            {
                ViewModelBuilderFactory.ResetBuildEntity(args?.EntityName);
            };

            EntityEvents.Entities.OnEntityDeleteField += (sender, args) =>
            {
                ViewModelBuilderFactory.ResetBuildEntity(args?.EntityName);
            };

            EntityEvents.Entities.OnEntityDeleted += (sender, args) =>
            {
                ViewModelBuilderFactory.ResetBuildEntity(args?.EntityName);
            };

            EntityEvents.Entities.OnEntityUpdateField += (sender, args) =>
            {
                ViewModelBuilderFactory.ResetBuildEntity(args?.EntityName);
            };

            EntityEvents.Entities.OnEntityUpdated += (sender, args) =>
            {
                ViewModelBuilderFactory.ResetBuildEntity(args?.EntityName);
            };

            return services;
        }
    }
}

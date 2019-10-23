using GR.Entities.Abstractions.Events;
using GR.Entities.Controls.Builders;
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

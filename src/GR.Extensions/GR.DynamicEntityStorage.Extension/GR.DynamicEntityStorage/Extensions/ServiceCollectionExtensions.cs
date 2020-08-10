using GR.Core;
using Microsoft.Extensions.DependencyInjection;
using GR.Core.Abstractions;
using GR.Core.Events;
using GR.Core.Helpers;
using GR.DynamicEntityStorage.Abstractions;
using GR.Entities.Abstractions;
using GR.Entities.Data;
using Microsoft.EntityFrameworkCore;

namespace GR.DynamicEntityStorage.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register new data access services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDynamicDataProviderModule<TContext>(this IServiceCollection services) where TContext : EntitiesDbContext
        {
            services.AddTransient<IDynamicService, DynamicService<TContext>>();
            services.AddTransient<IDynamicDataGetService, DynamicService<TContext>>();
            services.AddTransient<IDynamicDataCreateService, DynamicService<TContext>>();
            services.AddTransient<IDynamicDataUpdateService, DynamicService<TContext>>();

            IoC.RegisterTransientService<IDynamicService, DynamicService<TContext>>();

            //seed context table structure for dynamic management
            SystemEvents.Database.OnMigrateComplete += (sender, args) =>
           {
               if (!(args.DbContext is IDbContext)) return;
               GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueueToBeExecutedAfterAppInstalled(async (s, c) =>
               {
                   var tableService = s.GetService<ITablesService>();
                   var context = s.GetService<EntitiesDbContext>();
                   var synchronizer = s.GetService<EntitySynchronizer>();
                   var entities = tableService.GetEntitiesFromDbContexts(args.DbContext.GetType());

                   foreach (var ent in entities)
                   {
                       if (!await context.Table.AnyAsync(m => m.Name == ent.Name && m.TenantId == GearSettings.TenantId, c))
                       {
                           await synchronizer.SynchronizeEntities(ent, GearSettings.TenantId, ent.Schema);
                       }
                   }
               });

           };
            return services;
        }
    }
}

using GR.Core;
using Microsoft.Extensions.DependencyInjection;
using GR.Core.Abstractions;
using GR.Core.Events;
using GR.Core.Helpers;
using GR.DynamicEntityStorage.Abstractions;
using GR.DynamicEntityStorage.Abstractions.Seeders;
using GR.DynamicEntityStorage.Services;
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
            services.AddTransient<IDataFilter, DataFilter>();

            IoC.RegisterTransientService<IDynamicService, DynamicService<TContext>>();
            SystemEvents.Application.OnApplicationStarted += async (sender, args) =>
            {
                if (!GearApplication.Configured) return;
                var service = IoC.Resolve<IDynamicService>();

                await service.RegisterInMemoryDynamicTypesAsync();
                await NomenclatureManager.SyncNomenclaturesAsync();
            };

            //seed context table structure for dynamic management
            SystemEvents.Database.OnMigrateComplete += (sender, args) =>
           {
               if (!(args.DbContext is IDbContext)) return;
               GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueueToBeExecutedAfterAppInstalled(async x =>
               {
                   var tableService = IoC.Resolve<ITablesService>();
                   var context = IoC.Resolve<EntitiesDbContext>();
                   var synchronizer = IoC.Resolve<EntitySynchronizer>();
                   var entities = tableService.GetEntitiesFromDbContexts(args.DbContext.GetType());

                   foreach (var ent in entities)
                   {
                       if (!await context.Table.AnyAsync(s => s.Name == ent.Name && s.TenantId == GearSettings.TenantId, x))
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

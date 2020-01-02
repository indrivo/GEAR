using GR.Core;
using Microsoft.Extensions.DependencyInjection;
using GR.Core.Abstractions;
using GR.Core.Events;
using GR.Core.Helpers;
using GR.DynamicEntityStorage.Abstractions;
using GR.DynamicEntityStorage.Abstractions.Seeders;
using GR.DynamicEntityStorage.Services;
using GR.Entities.Data;

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
            SystemEvents.Database.OnMigrateComplete += async (sender, args) =>
            {
                //var service = IoC.Resolve<ITablesService>();
                if (!(args.DbContext is IDbContext)) return;
              
                //var entities = service.GetEntitiesFromDbContexts(args.DbContext.GetType());

                //foreach (var ent in entities)
                //{
                //    if (!await IoC.Resolve<EntitiesDbContext>().Table
                //        .AnyAsync(s => s.Name == ent.Name && s.TenantId == GearSettings.TenantId))
                //    {
                //        await IoC.Resolve<EntitySynchronizer>().SynchronizeEntities(ent, GearSettings.TenantId, ent.Schema);
                //    }
                //}
            };
            return services;
        }
    }
}

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
    public static class RegisterServiceCollection
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
                var service = IoC.Resolve<ITablesService>();
                if (!(args.DbContext is IDbContext)) return;
                return;
                var entities = service.GetEntitiesFromDbContexts(args.DbContext.GetType());

                foreach (var ent in entities)
                {
                    if (!await IoC.Resolve<EntitiesDbContext>().Table
                        .AnyAsync(s => s.Name == ent.Name && s.TenantId == GearSettings.TenantId))
                    {
                        await IoC.Resolve<EntitySynchronizer>().SynchronizeEntities(ent, GearSettings.TenantId, ent.Schema);
                    }
                }
            };
            return services;
        }
    }
}

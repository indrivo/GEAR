using GR.Core;
using Microsoft.Extensions.DependencyInjection;
using GR.Core.Abstractions;
using GR.Core.Events;
using GR.Core.Helpers;
using GR.DynamicEntityStorage.Abstractions;
using GR.DynamicEntityStorage.Services;
using GR.Entities.Data;

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
            };
            return services;
        }
    }
}

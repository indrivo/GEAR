using GR.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Cache.Abstractions.Extensions
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Use custom cache service
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCacheModule<TCacheService>(this IServiceCollection services)
            where TCacheService : class, ICacheService
        {
            services.AddGearSingleton<ICacheService, TCacheService>();
            return services;
        }
    }
}

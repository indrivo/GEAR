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
            services.AddSingleton<ICacheService, TCacheService>();
            return services;
        }

        /// <summary>
        /// Add generic cache module
        /// </summary>
        /// <typeparam name="TCacheService"></typeparam>
        /// <typeparam name="TCache"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGenericCacheModule<TCacheService, TCache>(this IServiceCollection services)
            where TCacheService : class, ICacheService<TCache>
        {
            services.AddSingleton<ICacheService<TCache>, TCacheService>();
            return services;
        }
    }
}

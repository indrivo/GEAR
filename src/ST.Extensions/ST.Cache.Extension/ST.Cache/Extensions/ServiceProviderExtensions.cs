using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ST.Cache.Abstractions;
using ST.Cache.Services;

namespace ST.Cache.Extensions
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Use custom cache service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="environment"></param>
        /// <param name="ip"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IServiceCollection UseCustomCacheService(this IServiceCollection services, IHostingEnvironment environment, string ip = "127.0.0.1", string instance = "ST.CORE")
        {
            services.AddDistributedRedisCache(opts =>
            {
                opts.Configuration = ip;
                opts.InstanceName = $"{instance}.{environment.EnvironmentName}@";
            });
            services.AddTransient<ICacheService, CacheService>();
            return services;
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ST.Cache.Abstractions;
using ST.Cache.Exceptions;
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
        /// <param name="configuration"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IServiceCollection UseCustomCacheService(this IServiceCollection services, IHostingEnvironment environment, IConfiguration configuration, string instance = "ST.CORE")
        {
            var redisSection = configuration.GetSection("RedisConnection");
            var redisConfig = redisSection.Get<RedisConnectionConfig>();
            if (redisConfig == null) throw new InvalidCacheConfigurationException();
            services.Configure<RedisConnectionConfig>(redisSection);
            services.AddDistributedRedisCache(opts =>
            {
                opts.Configuration = redisConfig.Host;
                opts.InstanceName = $"{instance}.{environment.EnvironmentName}@";
            });
            services.AddTransient<ICacheService, CacheService>();
            return services;
        }
    }
}

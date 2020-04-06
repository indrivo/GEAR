using System;
using GR.Cache.Abstractions.Models;
using GR.Cache.Exceptions;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Cache.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add cache module
        /// </summary>
        /// <param name="services"></param>
        /// <param name="environment"></param>
        /// <param name="configuration"></param>
        /// <param name="customSystemIdentifier"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedisCacheConfiguration<TRedisConnection>(this IServiceCollection services, IHostingEnvironment environment, IConfiguration configuration, string customSystemIdentifier = null)
            where TRedisConnection : class, IRedisConnection
        {
            if (customSystemIdentifier == null)
            {
                var systemIdentifier = configuration.GetSection(nameof(SystemConfig))
                    .GetValue<string>(nameof(SystemConfig.MachineIdentifier));

                if (string.IsNullOrEmpty(systemIdentifier))
                    throw new NullReferenceException("System identifier was not registered in appsettings file");
                customSystemIdentifier = systemIdentifier;
            }
            var redisSection = configuration.GetSection("RedisConnection");
            var redisConfig = redisSection.Get<CacheConfiguration>();
            if (redisConfig == null) throw new InvalidCacheConfigurationException();
            services.ConfigureWritable<CacheConfiguration>(configuration.GetSection("RedisConnection"));
            services.AddDistributedRedisCache(opts =>
            {
                opts.Configuration = redisConfig.Host;
                opts.InstanceName = $"{customSystemIdentifier}.{environment.EnvironmentName}@";
            });

            services.AddHealthChecks()
                .AddRedis(redisConfig.Host, name: "redis-cache", tags: new[]
                {
                    "cache", "redis", "gear"
                });

            services.AddSingleton<IRedisConnection, TRedisConnection>();
            IoC.RegisterSingletonService<IRedisConnection, TRedisConnection>();
            return services;
        }
    }
}

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GR.Cache.Abstractions;
using GR.Cache.Exceptions;
using GR.Cache.Services;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;

namespace GR.Cache.Extensions
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Use custom cache service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="environment"></param>
        /// <param name="configuration"></param>
        /// <param name="customSystemIdentifier"></param>
        /// <returns></returns>
        public static IServiceCollection AddCacheModule(this IServiceCollection services, IHostingEnvironment environment, IConfiguration configuration, string customSystemIdentifier = null)
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
            var redisConfig = redisSection.Get<RedisConnectionConfig>();
            if (redisConfig == null) throw new InvalidCacheConfigurationException();
            services.ConfigureWritable<RedisConnectionConfig>(configuration.GetSection("RedisConnection"));
            services.AddDistributedRedisCache(opts =>
            {
                opts.Configuration = redisConfig.Host;
                opts.InstanceName = $"{customSystemIdentifier}.{environment.EnvironmentName}@";
                //opts.ConfigurationOptions.SyncTimeout = 10000;
            });

            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<IRedisConnection, RedisConnection>();
            IoC.RegisterTransientService<ICacheService, CacheService>();
            IoC.RegisterTransientService<IRedisConnection, RedisConnection>();
            return services;
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using GR.Core.Abstractions;
using GR.Core.Helpers.Options;
using GR.Core.Services;

namespace GR.Core.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Register system config
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterSystemConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SystemConfig>(configuration.GetSection(nameof(SystemConfig)));
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            return services;
        }

        /// <summary>
        /// Writable options
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="section"></param>
        public static void ConfigureWritable<T>(
            this IServiceCollection services,
            IConfigurationSection section) where T : class, new()
        {
            services.Configure<T>(section);
            services.AddTransient<IWritableOptions<T>>(provider =>
            {
                var environment = provider.GetService<IHostingEnvironment>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                return new WritableOptions<T>(environment, options, section.Key);
            });
        }
    }
}

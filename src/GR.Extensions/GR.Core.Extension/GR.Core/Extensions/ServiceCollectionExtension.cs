using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using GR.Core.Abstractions;
using GR.Core.Helpers.Options;
using GR.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

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
            services.AddGearSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();
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

        /// <summary>
        /// Register background service
        /// </summary>
        /// <typeparam name="TBackgroundService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterBackgroundService<TBackgroundService>(this IServiceCollection services)
            where TBackgroundService : BaseBackgroundService<TBackgroundService>
        {
            services.AddHostedService<TBackgroundService>();
            return services;
        }

        /// <summary>
        /// Add url helper
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUrlHelper(this IServiceCollection services)
        {
            services.AddSingleton<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                    ?.ActionContext;
                return actionContext != null ? new UrlHelper(actionContext) : null;
            });
            return services;
        }
    }
}

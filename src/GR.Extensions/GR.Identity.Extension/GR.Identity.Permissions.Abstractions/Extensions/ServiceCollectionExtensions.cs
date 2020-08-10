using GR.Cache.Abstractions;
using GR.Core;
using GR.Core.Events;
using GR.Core.Helpers;
using GR.Identity.Permissions.Abstractions.Configurators;
using Microsoft.Extensions.DependencyInjection;
using System;
using GR.Core.Extensions;
using GR.Identity.Abstractions;
using GR.Identity.Permissions.Abstractions.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Permissions.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Authorization based on cache
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPermissionModule<TPermissionService>(this IServiceCollection services) where TPermissionService : class, IPermissionService
        {
            services.AddTransient<IPermissionService, TPermissionService>();
            IoC.RegisterTransientService<IPermissionService, TPermissionService>();
            SystemEvents.Application.OnApplicationStarted += async (sender, args) =>
            {
                if (!GearApplication.Configured) return;
                var cacheService = IoC.Resolve<ICacheService>();
                cacheService.FlushAll();

                var permissionService = IoC.Resolve<IPermissionService>();
                await permissionService.SetOrResetPermissionsOnCacheAsync();
            };

            return services;
        }

        /// <summary>
        /// Map permissions module
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection MapPermissionsModuleToContext<TContext>(this IServiceCollection services)
            where TContext : DbContext, IPermissionsContext, IIdentityContext
        {
            services.AddGearScoped<IPermissionsContext, TContext>();
            return services;
        }

        /// <summary>
        /// Register permissions for module
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <typeparam name="TPermissionsConstants"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterModulePermissionConfigurator<TConfiguration, TPermissionsConstants>(this IServiceCollection services, Action<PermissionConfig> options = null)
            where TConfiguration : DefaultPermissionsConfigurator<TPermissionsConstants>
            where TPermissionsConstants : class
        {
            var config = new PermissionConfig();
            options?.Invoke(config);

            if (config.SeedOnApplicationStarted)
            {
                SystemEvents.Application.OnApplicationStarted += (sender, args) =>
                {
                    GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async (serviceProvider, cancellationToken) =>
                    {
                        var instance = Activator.CreateInstance<TConfiguration>();
                        await instance.SeedAsync();
                    });
                };
            }
            else
            {
                var instance = Activator.CreateInstance<TConfiguration>();
                PermissionsProvider.Configurators.Enqueue(instance);
            }

            return services;
        }
    }
}
using GR.Cache.Abstractions;
using GR.Core;
using GR.Core.Events;
using GR.Core.Helpers;
using GR.Identity.Permissions.Abstractions.Configurators;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GR.Identity.Permissions.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Authorization based on cache
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPermissionService<TPermissionService>(this IServiceCollection services) where TPermissionService : class, IPermissionService
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
        /// Register permissions for module
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <typeparam name="TPermissionsConstants"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterModulePermissionConfigurator<TConfiguration, TPermissionsConstants>(this IServiceCollection services)
            where TConfiguration : DefaultPermissionsConfigurator<TPermissionsConstants>
            where TPermissionsConstants : class
        {
            SystemEvents.Database.OnSeed += async (sender, args) =>
            {
                var instance = Activator.CreateInstance<TConfiguration>();
                await instance.SeedAsync();
                instance.PermissionsSeedComplete();
            };

            return services;
        }
    }
}
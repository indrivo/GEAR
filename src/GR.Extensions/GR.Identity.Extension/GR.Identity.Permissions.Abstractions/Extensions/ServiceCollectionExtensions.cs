using System;
using GR.Core.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GR.Identity.Abstractions;
using GR.Identity.Permissions.Abstractions.Configurators;

namespace GR.Identity.Permissions.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Authorization based on cache
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorizationBasedOnCache<TContext, TPermissionService>(this IServiceCollection services)
            where TContext : DbContext, IIdentityContext
            where TPermissionService : class, IPermissionService
        {
            services.AddTransient<IPermissionService, TPermissionService>();
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
            SystemEvents.Application.OnApplicationStarted += async (sender, args) =>
            {
                var instance = Activator.CreateInstance<TConfiguration>();
                await instance.SeedAsync();
                instance.PermissionsSeedComplete();
            };

            return services;
        }
    }
}
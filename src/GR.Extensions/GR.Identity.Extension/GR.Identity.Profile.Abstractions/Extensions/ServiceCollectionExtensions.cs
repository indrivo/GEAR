using System;
using GR.Audit.Abstractions.Extensions;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Identity.Profile.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add profile module
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddProfileModule<TService>(this IServiceCollection services)
            where TService : class, IProfileService
        {
            services.AddGearTransient<IProfileService, TService>();
            return services;
        }

        /// <summary>
        /// Add profile storage module
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddProfileModuleStorage<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TContext : DbContext, IProfileContext
        {
            services.AddGearTransient<IProfileContext, TContext>();
            services.AddDbContext<TContext>(options);
            services.RegisterAuditFor<TContext>("Profile module");
            SystemEvents.Database.OnAllMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TContext>();
            };
            return services;
        }


        /// <summary>
        /// Register address service
        /// </summary>
        /// <typeparam name="TAddressService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUserAddressService<TAddressService>(this IServiceCollection services)
            where TAddressService : class, IUserAddressService
        {
            services.AddGearTransient<IUserAddressService, TAddressService>();
            return services;
        }
    }
}

using System;
using GR.Audit.Abstractions.Extensions;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Identity.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Identity.Groups.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add profile module
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUserGroupModule<TService, TUser>(this IServiceCollection services)
            where TService : class, IGroupService<TUser>
            where TUser : GearUser
        {
            services.AddGearTransient<IGroupService<TUser>, TService>();
            return services;
        }

        /// <summary>
        /// Add profile storage module
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddUserGroupModuleStorage<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TContext : DbContext, IGroupContext
        {
            services.AddGearTransient<IGroupContext, TContext>();
            services.AddDbContext<TContext>(options);
            services.RegisterAuditFor<TContext>("User Groups module");
            SystemEvents.Database.OnMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TContext>();
            };
            return services;
        }
    }
}

using System;
using GR.Audit.Abstractions.Extensions;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.UI.Menu.Abstractions.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GR.UI.Menu.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Menu service
        /// </summary>
        /// <typeparam name="TMenuService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMenuModule<TMenuService>(this IServiceCollection services)
            where TMenuService : class, IMenuService
        {
            services.AddGearScoped<IMenuService, TMenuService>();
            MenuEvents.RegisterEvents();
            return services;
        }

        /// <summary>
        /// Add module storage
        /// </summary>
        /// <typeparam name="TMenuContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddMenuModuleStorage<TMenuContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
            where TMenuContext : DbContext, IMenuDbContext
        {
            services.AddDbContext<TMenuContext>(options);
            services.AddScopedContextFactory<IMenuDbContext, TMenuContext>();
            services.RegisterAuditFor<IMenuDbContext>($"{nameof(Menu)} module");

            SystemEvents.Database.OnMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TMenuContext>();
            };
            return services;
        }
    }
}

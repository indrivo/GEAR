using System;
using GR.Audit.Abstractions.Extensions;
using GR.Core;
using Microsoft.Extensions.DependencyInjection;
using GR.Core.Abstractions;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Notifications.Abstractions.Helpers;
using GR.Notifications.Abstractions.Models.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GR.Notifications.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add notification module
        /// </summary>
        /// <typeparam name="TNotifyService"></typeparam>
        /// <typeparam name="TRole"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddNotificationModule<TNotifyService, TRole>(this IServiceCollection services)
            where TNotifyService : class, INotify<TRole>
            where TRole : IdentityRole<Guid>
        {
            services.AddGearTransient<INotify<TRole>, TNotifyService>();
            var appSender = IoC.Resolve<IAppSender>();
            appSender.RegisterProvider<TNotifyService>(NotificationResources.NotificationSender);
            return services;
        }

        /// <summary>
        /// Register seeder
        /// </summary>
        /// <typeparam name="TSeeder"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddNotificationSeeder<TSeeder>(this IServiceCollection services)
            where TSeeder : class, INotificationSeederService
        {
            services.AddGearScoped<INotificationSeederService, TSeeder>();
            return services;
        }

        /// <summary>
        /// Register module storage
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddNotificationModuleStorage<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TContext : DbContext, INotificationsContext
        {
            Arg.NotNull(services, nameof(AddNotificationModuleStorage));
            services.AddDbContext<TContext>(options);
            services.AddGearScoped<INotificationsContext, TContext>();
            services.RegisterAuditFor<INotificationsContext>($"{nameof(Notification)} module");
            SystemEvents.Database.OnAllMigrate += (sender, args) =>
            {
                GearApplication.GetHost().MigrateDbContext<TContext>();
            };
            return services;
        }
    }
}

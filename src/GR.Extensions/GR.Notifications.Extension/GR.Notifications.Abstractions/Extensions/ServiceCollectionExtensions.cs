using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GR.Audit.Abstractions.Extensions;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using GR.Notifications.Abstractions.ServiceBuilder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

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
            where TRole : IdentityRole<string>
        {
            services.AddGearTransient<INotify<TRole>, TNotifyService>();

            return services;
        }

        /// <summary>
        /// Add notification subscriptions module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static INotificationSubscriptionServiceCollection AddNotificationSubscriptionModule<TRepository>(this IServiceCollection services)
            where TRepository : class, INotificationSubscriptionRepository
        {
            services.AddGearScoped<INotificationSubscriptionRepository, TRepository>();
            return new NotificationSubscriptionServiceCollection(services);
        }

        /// <summary>
        /// Register module storage
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static INotificationSubscriptionServiceCollection AddNotificationSubscriptionModuleStorage<TContext>(this INotificationSubscriptionServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TContext : DbContext, INotificationDbContext
        {
            Arg.NotNull(services, nameof(AddNotificationSubscriptionModuleStorage));
            services.Services.AddDbContext<TContext>(options);
            services.Services.AddScopedContextFactory<INotificationDbContext, TContext>();
            services.Services.RegisterAuditFor<INotificationDbContext>($"{nameof(Notification)} module");
            SystemEvents.Database.OnMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TContext>();
            };
            return services;
        }

        /// <summary>
        /// Register notificator notification sender
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static INotificationSubscriptionServiceCollection AddNotificationModuleEvents(this INotificationSubscriptionServiceCollection services)
        {
            SystemEvents.Application.OnEvent += (obj, args) =>
           {
               if (!GearApplication.Configured) return;
               GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
               {
                   try
                   {
                       if (string.IsNullOrEmpty(args.EventName)) return;
                       var service = IoC.Resolve<INotificationSubscriptionRepository>();
                       var notifier = IoC.Resolve<INotify<GearRole>>();
                       var subscribedRoles = await service.GetRolesSubscribedToEventAsync(args.EventName);
                       if (!subscribedRoles.IsSuccess) return;
                       var template = await service.GetEventTemplateAsync(args.EventName);
                       if (!template.IsSuccess) return;
                       var templateWithParams = template.Result.Value?.Inject(args.EventArgs);
                       //var engine = new RazorLightEngineBuilder()
                       //    .UseMemoryCachingProvider()
                       //    .Build();

                       //var templateWithParams = await engine.CompileRenderAsync($"template_{ev.EventName}", template.Result.Value, ev.EventArgs);

                       var notification = new Notification
                       {
                           Subject = template.Result.Subject,
                           Content = templateWithParams,
                           NotificationTypeId = NotificationType.Info
                       };

                       await notifier.SendNotificationAsync(subscribedRoles.Result, notification, null);
                   }
                   catch (Exception e)
                   {
                       Console.WriteLine(e);
                   }
               });
           };

            SystemEvents.Database.OnSeed += async (obj, args) =>
            {
                if (!(args.DbContext is INotificationDbContext)) return;
                try
                {
                    var service = IoC.Resolve<INotificationSubscriptionRepository>();
                    await service.SeedEventsAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            };

            return services;
        }
    }
}

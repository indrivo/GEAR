using System;
using GR.Audit.Abstractions.Extensions;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Notifications.Subscriptions.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add notification subscriptions module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddNotificationSubscriptionModule<TRepository>(this IServiceCollection services)
            where TRepository : class, INotificationSubscriptionService
        {
            services.AddGearScoped<INotificationSubscriptionService, TRepository>();
            return services;
        }

        /// <summary>
        /// Register module storage
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddNotificationSubscriptionModuleStorage<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TContext : DbContext, INotificationSubscriptionsDbContext
        {
            Arg.NotNull(services, nameof(AddNotificationSubscriptionModuleStorage));
            services.AddDbContext<TContext>(options);
            services.AddScopedContextFactory<INotificationSubscriptionsDbContext, TContext>();
            services.RegisterAuditFor<INotificationSubscriptionsDbContext>($"{nameof(Notification)} subscription module");
            SystemEvents.Database.OnAllMigrate += (sender, args) =>
            {
                GearApplication.GetHost().MigrateDbContext<TContext>();
            };
            return services;
        }

        /// <summary>
        /// Register notificator notification sender
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddNotificationModuleEvents(this IServiceCollection services)
        {
            SystemEvents.Application.OnEvent += (obj, args) =>
            {
                if (!GearApplication.Configured) return;
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async (serviceProvider, cancellationToken) =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(args.EventName)) return;
                        var service = IoC.Resolve<INotificationSubscriptionService>();
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
                if (!(args.DbContext is INotificationSubscriptionsDbContext)) return;
                try
                {
                    var service = IoC.Resolve<INotificationSubscriptionService>();
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

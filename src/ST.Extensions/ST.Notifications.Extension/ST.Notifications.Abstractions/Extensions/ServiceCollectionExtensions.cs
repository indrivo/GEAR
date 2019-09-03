using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Audit.Abstractions.Extensions;
using ST.Core.Events;
using ST.Core.Events.EventArgs;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Identity.Abstractions;
using ST.Notifications.Abstractions.Models.Notifications;
using ST.Notifications.Abstractions.ServiceBuilder;

namespace ST.Notifications.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add notification subscriptions module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static INotificationSubscriptionServiceCollection AddNotificationSubscriptionModule<TRepository>(this IServiceCollection services)
            where TRepository : class, INotificationSubscriptionRepository
        {
            Arg.NotNull(services, nameof(AddNotificationSubscriptionModule));
            IoC.RegisterScopedService<INotificationSubscriptionRepository, TRepository>();
            services.AddTransient<INotificationSubscriptionRepository, TRepository>();
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
            services.Services.RegisterAuditFor<INotificationDbContext>("Notification module");
            return services;
        }

        /// <summary>
        /// Register notificator notification sender
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static INotificationSubscriptionServiceCollection AddNotificationModuleEvents(this INotificationSubscriptionServiceCollection services)
        {
            SystemEvents.Application.OnEvent += async delegate (object sender, ApplicationEventEventArgs ev)
            {
                try
                {
                    if (string.IsNullOrEmpty(ev.EventName)) return;
                    var service = IoC.Resolve<INotificationSubscriptionRepository>();
                    var notifier = IoC.Resolve<INotify<ApplicationRole>>();
                    var subscribedRoles = await service.GetRolesSubscribedToEventAsync(ev.EventName);
                    if (!subscribedRoles.IsSuccess) return;
                    var template = await service.GetEventTemplateAsync(ev.EventName);
                    if (!template.IsSuccess) return;
                    var templateWithParams = template.Result.Value?.Inject(ev.EventArgs);
                    //var engine = new RazorLightEngineBuilder()
                    //    .UseMemoryCachingProvider()
                    //    .Build();

                    //var templateWithParams = await engine.CompileRenderAsync($"template_{ev.EventName}", template.Result.Value, ev.EventArgs);

                    var notification = new SystemNotifications
                    {
                        Subject = template.Result.Subject,
                        Content = templateWithParams,
                        NotificationTypeId = NotificationType.Info
                    };

                    await notifier.SendNotificationAsync(subscribedRoles.Result, notification);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            };

            //seed events
            SystemEvents.Application.OnApplicationStarted += async delegate
            {
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

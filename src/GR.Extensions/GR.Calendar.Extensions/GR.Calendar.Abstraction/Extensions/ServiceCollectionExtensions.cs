using System;
using System.Collections.Generic;
using System.Linq;
using GR.Audit.Abstractions.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GR.Calendar.Abstractions.BackGroundServices;
using GR.Calendar.Abstractions.Events;
using GR.Calendar.Abstractions.Helpers.ServiceBuilders;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;


namespace GR.Calendar.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register calendar module
        /// </summary>
        /// <typeparam name="TCalendarService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static CalendarServiceCollection AddCalendarModule<TCalendarService>(this IServiceCollection services)
            where TCalendarService : class, ICalendarManager
        {
            Arg.NotNull(services, nameof(AddCalendarModule));
            services.AddGearTransient<ICalendarManager, TCalendarService>();
            services.AddHostedService<EventReminderBackgroundService>();
            return new CalendarServiceCollection(services);
        }

        /// <summary>
        /// Add calendar module storage
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static CalendarServiceCollection AddCalendarModuleStorage<TDbContext>(
            this CalendarServiceCollection configuration, Action<DbContextOptionsBuilder> options)
            where TDbContext : DbContext, ICalendarDbContext
        {
            Arg.NotNull(configuration.Services, nameof(AddCalendarModuleStorage));
            configuration.Services.AddDbContext<TDbContext>(options, ServiceLifetime.Transient);
            configuration.Services.AddScopedContextFactory<ICalendarDbContext, TDbContext>();
            configuration.Services.RegisterAuditFor<ICalendarDbContext>($"{nameof(Calendar)} module");
            SystemEvents.Database.OnMigrate += (sender, args) =>
                {
                    GearApplication.GetHost<IWebHost>().MigrateDbContext<TDbContext>();
                };
            return configuration;
        }


        /// <summary>
        /// Register runtime events
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static CalendarServiceCollection AddCalendarRuntimeEvents(this CalendarServiceCollection serviceCollection)
        {
            CalendarEvents.RegisterEvents();
            const string calendarLink = "<a href='/calendar'>here</a>";
            CalendarEvents.SystemCalendarEvents.OnEventCreated += async (sender, args) =>
            {
                var subject = "Changes in the event " + args.Title;
                var message = $"An event is created for which you are invited, more details {calendarLink}";
                var notifier = IoC.Resolve<INotify<GearRole>>();
                if (notifier == null) return;
                var users = args.Invited?.Select(x => x.ToGuid()).ToList() ?? new List<Guid>();
                if (users.Any())
                {
                    await notifier.SendNotificationAsync(users, NotificationType.Info, subject, message);
                }
            };

            CalendarEvents.SystemCalendarEvents.OnEventUpdated += async (sender, args) =>
            {
                var subject = "Changes in the event " + args.Title;
                var message = $"Event {args.Title} has been modified, more details {calendarLink}";
                var notifier = IoC.Resolve<INotify<GearRole>>();
                var users = args.Invited?.Select(x => x.ToGuid()).ToList() ?? new List<Guid>();
                if (users.Any())
                {
                    await notifier.SendNotificationAsync(users, NotificationType.Info, subject, message);
                }
            };

            CalendarEvents.SystemCalendarEvents.OnEventDeleted += async (sender, args) =>
            {
                var subject = "Changes in the event " + args.Title;
                var message = $"Event {args.Title} was canceled";
                var notifier = IoC.Resolve<INotify<GearRole>>();
                var users = args.Invited?.Select(x => x.ToGuid()).ToList() ?? new List<Guid>();
                if (users.Any())
                {
                    await notifier.SendNotificationAsync(users, NotificationType.Info, subject, message);
                }
            };

            CalendarEvents.SystemCalendarEvents.OnUserChangeAcceptance += async (sender, args) =>
            {
                var userManager = IoC.Resolve<UserManager<GearUser>>();
                if (userManager == null) return;
                var subject = "Changes in the event " + args.Title;
                var user = await userManager.FindByIdAsync(args.Member.UserId.ToString());
                var message = $"User {user.Email} responded with {args.AcceptanceState} to event {args.Title}";
                var notifier = IoC.Resolve<INotify<GearRole>>();
                await notifier.SendNotificationAsync(new List<Guid> { args.Organizer }, NotificationType.Info, subject, message);
            };

            return serviceCollection;
        }

        /// <summary>
        /// Register token provider
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static CalendarServiceCollection RegisterTokenProvider<TProvider>(this CalendarServiceCollection serviceCollection)
            where TProvider : class, ICalendarExternalTokenProvider
        {
            IoC.RegisterTransientService<ICalendarExternalTokenProvider, TProvider>();
            return serviceCollection;
        }

        /// <summary>
        /// Register calendar
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static CalendarServiceCollection RegisterCalendarUserPreferencesProvider<TService>(this CalendarServiceCollection serviceCollection)
            where TService : class, ICalendarUserSettingsService
        {
            IoC.RegisterTransientService<ICalendarUserSettingsService, TService>();
            return serviceCollection;
        }
    }
}

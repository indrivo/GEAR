using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using GR.Calendar.Abstractions.Helpers.ServiceBuilders;
using GR.Calendar.Razor.Helpers;
using GR.Core;
using GR.Core.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;

namespace GR.Calendar.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register page render
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCalendarRazorUIModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(InternalCalendarFileConfiguration));

            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
                {
                    var service = IoC.Resolve<IMenuService>();
                    await service.AppendMenuItemsAsync(new CalendarMenuInitializer());
                });
            };
            return services;
        }

        /// <summary>
        /// Set serialize format
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection SetSerializationFormatSettings(this IServiceCollection services,
            Action<JsonSerializerSettings> options)
        {
            options.Invoke(CalendarServiceCollection.JsonSerializerSettings);
            CalendarServiceCollection.JsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return services;
        }
    }
}

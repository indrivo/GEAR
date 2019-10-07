using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ST.Calendar.Abstractions.Helpers.ServiceBuilders;
using ST.Calendar.Razor.Helpers;

namespace ST.Calendar.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register page render
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static CalendarServiceCollection AddCalendarRazorUIModule(this CalendarServiceCollection services)
        {
            services.Services.ConfigureOptions(typeof(InternalCalendarFileConfiguration));
            return services;
        }

        /// <summary>
        /// Set serialize format
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static CalendarServiceCollection SetSerializationFormatSettings(this CalendarServiceCollection services,
            Action<JsonSerializerSettings> options)
        {
            options.Invoke(CalendarServiceCollection.JsonSerializerSettings);
            CalendarServiceCollection.JsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return services;
        }
    }
}

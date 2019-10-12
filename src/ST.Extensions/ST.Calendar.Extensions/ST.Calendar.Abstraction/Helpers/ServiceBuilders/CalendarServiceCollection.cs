using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ST.Calendar.Abstractions.Helpers.ServiceBuilders
{
    public class CalendarServiceCollection
    {
        /// <summary>
        /// Services
        /// </summary>
        public IServiceCollection Services;

        /// <summary>
        /// Json settings
        /// </summary>
        public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings();

        public CalendarServiceCollection(IServiceCollection services)
        {
            Services = services;
        }
    }
}

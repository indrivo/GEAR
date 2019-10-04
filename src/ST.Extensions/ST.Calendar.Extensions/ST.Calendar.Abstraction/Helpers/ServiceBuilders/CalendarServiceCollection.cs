using Microsoft.Extensions.DependencyInjection;

namespace ST.Calendar.Abstractions.Helpers.ServiceBuilders
{
    public class CalendarServiceCollection
    {
        /// <summary>
        /// Services
        /// </summary>
        public IServiceCollection Services;

        public CalendarServiceCollection(IServiceCollection services)
        {
            Services = services;
        }
    }
}

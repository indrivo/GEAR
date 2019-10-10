using ST.Calendar.Abstractions.Helpers.ServiceBuilders;
using ST.Calendar.Abstractions.ExternalProviders.Extensions;

namespace ST.Calendar.Providers.Google.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register google calendar
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static CalendarServiceCollection RegisterGoogleCalendarProvider(this CalendarServiceCollection serviceCollection)
        {
            serviceCollection.RegisterExternalCalendarProvider(options =>
            {
                options.ProviderName = nameof(GoogleCalendarProvider);
                options.ProviderType = typeof(GoogleCalendarProvider);
            });

            return serviceCollection;
        }
    }
}

using GR.Calendar.Abstractions.Helpers.ServiceBuilders;
using GR.Calendar.Abstractions.ExternalProviders.Extensions;

namespace GR.Calendar.Providers.Google.Extensions
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
                options.DisplayName = "Google";
                options.FontAwesomeIcon = "google";
            });

            return serviceCollection;
        }
    }
}

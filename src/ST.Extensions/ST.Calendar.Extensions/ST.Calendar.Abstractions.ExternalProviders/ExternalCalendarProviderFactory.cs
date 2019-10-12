using System.Collections.Generic;
using ST.Calendar.Abstractions.ExternalProviders.Exceptions;
using ST.Calendar.Abstractions.ExternalProviders.Helpers;
using ST.Core.Extensions;
using ST.Core.Helpers;

namespace ST.Calendar.Abstractions.ExternalProviders
{
    public class ExternalCalendarProviderFactory
    {
        /// <summary>
        /// Create service
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public IExternalCalendarProvider CreateService(string providerName)
        {
            if (providerName.IsNullOrEmpty()) throw new InvalidCalendarProviderNameException(providerName);
            return IoC.Resolve<IExternalCalendarProvider>(providerName);
        }

        /// <summary>
        /// Get providers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetProviders()
        {
            return CalendarProviders.GetAllProviders();
        }

        /// <summary>
        /// Get providers info
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ExternalProviderConfig> GetProvidersInfo()
        {
            return CalendarProviders.GetProvidersInfo();
        }
    }
}

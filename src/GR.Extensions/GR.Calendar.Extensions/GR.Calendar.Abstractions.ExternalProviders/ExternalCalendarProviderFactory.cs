using System.Collections.Generic;
using GR.Calendar.Abstractions.ExternalProviders.Exceptions;
using GR.Calendar.Abstractions.ExternalProviders.Helpers;
using GR.Core.Extensions;
using GR.Core.Helpers;

namespace GR.Calendar.Abstractions.ExternalProviders
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

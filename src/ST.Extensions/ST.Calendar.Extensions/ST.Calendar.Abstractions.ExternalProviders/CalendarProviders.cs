using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ST.Calendar.Abstractions.ExternalProviders.Helpers;

namespace ST.Calendar.Abstractions.ExternalProviders
{
    internal static class CalendarProviders
    {
        /// <summary>
        /// Providers
        /// </summary>
        private static ConcurrentQueue<ExternalProviderConfig> Providers { get; set; } = new ConcurrentQueue<ExternalProviderConfig>();

        /// <summary>
        /// Register provider
        /// </summary>
        /// <param name="provider"></param>
        public static void RegisterProviderInMemory(ExternalProviderConfig provider)
        {
            if (!Providers.Select(x => x.ProviderName).Contains(provider.ProviderName))
                Providers.Enqueue(provider);
        }

        /// <summary>
        /// Retrieve provider list
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAllProviders()
        {
            return Providers.Select(x => x.ProviderName).ToList();
        }

        /// <summary>
        /// Get providers
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ExternalProviderConfig> GetProvidersInfo()
        {
            return Providers.ToList();
        }
    }
}

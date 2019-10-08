using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ST.Calendar.Abstractions.ExternalProviders
{
    internal static class CalendarProviders
    {
        /// <summary>
        /// Providers
        /// </summary>
        private static ConcurrentQueue<string> Providers { get; set; } = new ConcurrentQueue<string>();

        /// <summary>
        /// Register provider
        /// </summary>
        /// <param name="providerName"></param>
        public static void RegisterProviderInMemory(string providerName)
        {
            if (!Providers.Contains(providerName))
                Providers.Enqueue(providerName);
        }

        /// <summary>
        /// Retrieve provider list
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAllProviders()
        {
            return Providers.ToList();
        }
    }
}

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GR.Core.Extensions;

namespace GR.ECommerce.Payments.Abstractions.Storage
{
    public static class PaymentProviders
    {
        /// <summary>
        /// Providers
        /// </summary>
        public static ConcurrentQueue<string> Providers { get; set; } = new ConcurrentQueue<string>();

        /// <summary>
        /// Check if exist
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static bool Exist(string provider)
        {
            return !provider.IsNullOrEmpty() && Providers.Any(x => x.Equals(provider));
        }

        /// <summary>
        /// Get all providers
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetProviders() => Providers.ToList();

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="provider"></param>
        public static void Register(string provider)
        {
            if (!Providers.Contains(provider))
                Providers.Enqueue(provider);
        }
    }
}

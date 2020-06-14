using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions.Configurator;
using GR.ECommerce.Payments.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Payments.Abstractions.Helpers
{
    internal static class PaymentProviders
    {
        /// <summary>
        /// Collection of payment providers
        /// </summary>
        private static readonly ConcurrentDictionary<string, PaymentProvider> Providers = new ConcurrentDictionary<string, PaymentProvider>();

        /// <summary>
        /// Check if provider is registered
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static bool IsRegistered(string providerName)
        {
            if (providerName.IsNullOrEmpty())
                throw new NullReferenceException($"parameter {nameof(providerName)} is required");
            return Providers.Any(x => x.Key.Equals(providerName));
        }

        /// <summary>
        /// Register new provider
        /// </summary>
        /// <param name="name"></param>
        /// <param name="conf"></param>
        public static void Register(string name, PaymentProvider conf)
        {
            Arg.NotNull(conf, nameof(Register));
            if (IsRegistered(name)) throw new Exception($"Provider {name} already exists");
            Providers.TryAdd(name, conf);
        }

        /// <summary>
        /// Get provider info
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static PaymentProvider GetProviderInfo(string providerName)
        {
            return !IsRegistered(providerName)
                ? default
                : Providers.FirstOrDefault(x => x.Key.Equals(providerName)).Value;
        }

        /// <summary>
        /// Get payment providers
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PaymentProvider> GetPaymentProviders()
            => Providers.Select(x => x.Value).ToList();

        /// <summary>
        /// Seed provider
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns></returns>
        public static async Task SeedProviderAsync(string providerId)
        {
            var context = IoC.Resolve<IPaymentContext>();
            if (await context.PaymentMethods.AnyAsync(x => x.Name == providerId)) return;
            var paymentMethod = new PaymentMethod
            {
                Name = providerId,
                IsEnabled = false
            };
            await context.PaymentMethods.AddAsync(paymentMethod);
            context.PushAsync().Wait();
        }
    }
}
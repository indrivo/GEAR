using GR.ECommerce.Payments.Abstractions.Configurator;
using GR.ECommerce.Payments.Abstractions.Extensions;
using GR.GooglePay.Abstractions.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace GR.GooglePay.Abstractions.Extensions
{
    public static class GPayConfigurationExtensions
    {
        /// <summary>
        /// Register paypal provider
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterGPayProvider<TPayPalProvider>(this IServiceCollection services)
            where TPayPalProvider : class, IGPayPaymentMethodService
        {
            services.RegisterPaymentProvider(new PaymentProvider<IGPayPaymentMethodService, TPayPalProvider>
            {
                Id = GPayResources.GPayProvider,
                DisplayName = "Google Pay",
                Description = "Google Pay is a digital wallet platform and online payment system developed by Google to power in-app and tap-to-pay purchases"
            });

            return services;
        }
    }
}
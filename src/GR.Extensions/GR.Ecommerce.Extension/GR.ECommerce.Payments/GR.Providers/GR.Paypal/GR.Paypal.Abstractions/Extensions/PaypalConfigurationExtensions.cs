using GR.ECommerce.Payments.Abstractions.Configurator;
using GR.ECommerce.Payments.Abstractions.Extensions;
using Microsoft.Extensions.DependencyInjection;
using GR.Paypal.Abstractions.Helpers;

namespace GR.Paypal.Abstractions.Extensions
{
    public static class PaypalConfigurationExtensions
    {
        /// <summary>
        /// Register paypal provider
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterPaypalProvider<TPayPalProvider>(this IServiceCollection services)
            where TPayPalProvider : class, IPaypalPaymentMethodService
        {
            services.RegisterPaymentProvider(new PaymentProvider<IPaypalPaymentMethodService, TPayPalProvider>
            {
                Id = PaypalResources.PayPalProvider,
                DisplayName = PaypalResources.PayPalProvider,
                Description = "Paypal is an American company operating a worldwide online payments"
            });

            return services;
        }
    }
}
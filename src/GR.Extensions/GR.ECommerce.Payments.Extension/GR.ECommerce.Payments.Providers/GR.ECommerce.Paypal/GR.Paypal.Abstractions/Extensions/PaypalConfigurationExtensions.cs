using GR.ECommerce.Payments.Abstractions.Configurator;
using GR.ECommerce.Payments.Abstractions.Extensions;
using GR.ECommerce.Paypal.Abstractions;
using Microsoft.Extensions.DependencyInjection;

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
            where TPayPalProvider : class, IPaypalPaymentService
        {
            services.RegisterPaymentProvider(new PaymentProvider<TPayPalProvider>());
            return services;
        }
    }
}
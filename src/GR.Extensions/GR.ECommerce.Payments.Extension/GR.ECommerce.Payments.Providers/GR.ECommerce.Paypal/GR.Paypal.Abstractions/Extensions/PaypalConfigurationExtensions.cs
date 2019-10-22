using GR.ECommerce.Payments.Abstractions.Configurator;
using GR.ECommerce.Payments.Abstractions.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace GR.ECommerce.Paypal.Abstractions.Extensions
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
            services.AddHttpClient();
            services.RegisterPaymentProvider(new PaymentProvider<TPayPalProvider>());
            return services;
        }
    }
}
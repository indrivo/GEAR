using GR.ECommerce.Payments.Abstractions.Configurator;
using GR.ECommerce.Payments.Abstractions.Extensions;
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
            services.RegisterPaymentProvider(new PaymentProvider<TPayPalProvider>
            {
                DisplayName = "Paypal",
                Description = "Paypal is an American company operating a worldwide online payments"
            });
            return services;
        }
    }
}
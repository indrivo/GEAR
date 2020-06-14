using GR.Braintree.Abstractions.Helpers;
using GR.ECommerce.Payments.Abstractions.Configurator;
using GR.ECommerce.Payments.Abstractions.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Braintree.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register Braintree
        /// </summary>
        /// <typeparam name="TBraintreeProvider"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterBraintreeProvider<TBraintreeProvider>(this IServiceCollection services)
            where TBraintreeProvider : class, IBraintreePaymentMethod
        {
            services.RegisterPaymentProvider(new PaymentProvider<IBraintreePaymentMethod, TBraintreeProvider>
            {
                Id = BraintreeResources.BraintreeProvider,
                DisplayName = BraintreeResources.BraintreeProvider,
                Description = "Integrating with Braintree offers your customers many different ways to pay. These payment methods can be added to your checkout. " +
                              "Each payment method differs in availability, configuration, and implementation."
            });

            return services;
        }
    }
}
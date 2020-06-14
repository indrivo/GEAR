using GR.ApplePay.Abstractions.Helpers;
using GR.ECommerce.Payments.Abstractions.Configurator;
using GR.ECommerce.Payments.Abstractions.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace GR.ApplePay.Abstractions.Extensions
{
    public static class ApplePayConfigurationExtensions
    {
        /// <summary>
        /// Register apple provider
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterApplePayProvider<TAppleProvider>(this IServiceCollection services)
            where TAppleProvider : class, IApplePayPaymentMethodService
        {
            services.RegisterPaymentProvider(new PaymentProvider<IApplePayPaymentMethodService, TAppleProvider>
            {
                Id = ApplePayResources.ApplePayProvider,
                DisplayName = "Apple Pay",
                Description = "Apple Pay is a mobile payment and digital wallet service by Apple Inc. that allows users to make payments in person, in iOS apps, and on the web."
            });

            return services;
        }
    }
}
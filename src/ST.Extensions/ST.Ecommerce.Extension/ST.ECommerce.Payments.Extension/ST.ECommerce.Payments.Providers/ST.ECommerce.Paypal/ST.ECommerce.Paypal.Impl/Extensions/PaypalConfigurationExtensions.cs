using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using ST.ECommerce.Payments.Abstractions.Configurator;
using ST.ECommerce.Payments.Abstractions.Extensions;

namespace ST.ECommerce.Paypal.Impl.Extensions
{
    public static class PaypalConfigurationExtensions
    {
        /// <summary>
        /// Register paypal provider
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterPaypalProvider(this IServiceCollection services)
        {
            services.RegisterPaymentProvider(new PaymentProvider<PaypalPaymentManager>());
            
            services.ConfigureOptions(typeof(PaypalAssetsRazorFileConfiguration));
            return services;
        }
    }
}

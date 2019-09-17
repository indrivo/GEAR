using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using ST.ECommerce.Paypal.Razor.Helpers;

namespace ST.ECommerce.Paypal.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register paypal provider
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterPaypalRazorProvider(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(PaypalAssetsRazorFileConfiguration));
            return services;
        }
    }
}

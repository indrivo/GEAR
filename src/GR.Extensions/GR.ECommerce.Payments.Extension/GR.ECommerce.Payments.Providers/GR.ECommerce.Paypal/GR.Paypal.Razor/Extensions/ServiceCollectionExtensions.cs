using GR.Core.Extensions;
using GR.ECommerce.Paypal.Models;
using GR.Paypal.Razor.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Paypal.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register paypal provider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterPaypalRazorProvider(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigureOptions(typeof(PaypalAssetsRazorFileConfiguration));

            services.ConfigureWritable<PaypalExpressConfigForm>(configuration.GetSection("PayPalSettings"));
            return services;
        }
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Extensions;
using ST.ECommerce.Paypal.Razor.Helpers;
using ST.ECommerce.Paypal.Razor.ViewModels;

namespace ST.ECommerce.Paypal.Razor.Extensions
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
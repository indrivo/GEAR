using GR.Braintree.Abstractions.Models;
using GR.Braintree.Razor.Helpers;
using GR.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Braintree.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register mobil pay provider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterBraintreeRazorProvider(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigureOptions(typeof(BraintreeAssetsRazorFileConfiguration));
            services.ConfigureWritable<BraintreeConfiguration>(configuration.GetSection("BraintreeSettings"));
            return services;
        }
    }
}
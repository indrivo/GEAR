using GR.Core.Extensions;
using GR.MobilPay.Abstractions.Models;
using GR.MobilPay.Razor.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GR.MobilPay.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register mobil pay provider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterMobilPayRazorProvider(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigureOptions(typeof(MobilPayAssetsRazorFileConfiguration));

            services.ConfigureWritable<MobilPayConfiguration>(configuration.GetSection("MobilPaySettings"));
            return services;
        }
    }
}
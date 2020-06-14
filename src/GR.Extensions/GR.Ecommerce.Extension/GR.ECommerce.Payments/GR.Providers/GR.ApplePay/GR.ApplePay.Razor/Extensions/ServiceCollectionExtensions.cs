using GR.ApplePay.Abstractions.ViewModels;
using GR.ApplePay.Razor.Helpers;
using GR.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GR.ApplePay.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ApplePay razor provider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterApplePayRazorProvider(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigureOptions(typeof(ApplePayRazorFileConfiguration));
            services.ConfigureWritable<ApplePaySettingsViewModel>(configuration.GetSection("ApplePaySettings"));
            return services;
        }
    }
}
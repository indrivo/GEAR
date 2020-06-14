using GR.Core.Extensions;
using GR.GooglePay.Abstractions.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GR.GooglePay.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register GPay razor provider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterGPayRazorProvider(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigureWritable<GPaySettingsViewModel>(configuration.GetSection("GPaySettings"));
            return services;
        }
    }
}
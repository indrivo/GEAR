using GR.Card.Abstractions.Models;
using GR.Card.Razor.Helpers;
using GR.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Card.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register paypal provider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterCreditCardRazorProvider(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigureOptions(typeof(CardAssetsRazorFileConfiguration));
            services.ConfigureWritable<CardSettingsViewModel>(configuration.GetSection("CardSettings"));
            return services;
        }
    }
}
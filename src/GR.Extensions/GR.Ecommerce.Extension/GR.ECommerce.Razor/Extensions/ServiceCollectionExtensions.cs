using GR.ECommerce.Razor.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace GR.ECommerce.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCommerceRazorUIModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(CommerceRazorFileConfiguration));
            return services;
        }
    }
}

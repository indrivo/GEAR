using Microsoft.Extensions.DependencyInjection;
using GR.Email.Razor.Helpers;

namespace GR.Email.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEmailRazorUIModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(EmailRazorFileConfiguration));
            return services;
        }
    }
}

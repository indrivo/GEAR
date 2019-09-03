using Microsoft.Extensions.DependencyInjection;
using ST.Email.Razor.Helpers;

namespace ST.Email.Razor.Extensions
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

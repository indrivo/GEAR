using Microsoft.Extensions.DependencyInjection;
using ST.MultiTenant.Razor.Helpers;

namespace ST.MultiTenant.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMultiTenantRazorUIModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(MultiTenantRazorFileConfiguration));
            return services;
        }
    }
}

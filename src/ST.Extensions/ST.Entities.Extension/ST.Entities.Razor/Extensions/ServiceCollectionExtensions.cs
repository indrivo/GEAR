using Microsoft.Extensions.DependencyInjection;
using ST.Entities.Razor.Helpers;

namespace ST.Entities.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEntityRazorUIModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(EntityRazorFileConfiguration));
            return services;
        }
    }
}

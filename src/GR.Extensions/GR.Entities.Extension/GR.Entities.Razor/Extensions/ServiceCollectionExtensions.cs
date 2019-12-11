using Microsoft.Extensions.DependencyInjection;
using GR.Entities.Razor.Helpers;

namespace GR.Entities.Razor.Extensions
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

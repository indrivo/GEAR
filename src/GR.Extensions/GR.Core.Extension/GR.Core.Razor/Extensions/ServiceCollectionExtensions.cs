using GR.Core.Razor.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Core.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register core razor module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterCoreRazorModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(CoreRazorFileConfiguration));
            return services;
        }
    }
}

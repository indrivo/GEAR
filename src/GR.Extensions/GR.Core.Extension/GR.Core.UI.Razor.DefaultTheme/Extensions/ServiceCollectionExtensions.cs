using GR.Core.UI.Razor.DefaultTheme.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Core.UI.Razor.DefaultTheme.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register core razor module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterDefaultThemeRazorModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(DefaultThemeRazorFileConfiguration));
            return services;
        }
    }
}

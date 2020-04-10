using Microsoft.Extensions.DependencyInjection;
using GR.Process.Razor.Helpers;

namespace GR.Process.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register processes dependencies 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddProcessesRazorModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(ProcessesFileConfiguration));
            return services;
        }
    }
}

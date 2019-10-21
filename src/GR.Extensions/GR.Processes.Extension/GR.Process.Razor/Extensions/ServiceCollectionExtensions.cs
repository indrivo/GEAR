using Microsoft.Extensions.DependencyInjection;
using GR.Procesess.Abstraction;
using GR.Procesess.Parsers;
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
        public static IServiceCollection AddProcessesModule(this IServiceCollection services)
        {
            services.AddTransient<IProcessParser, ProcessParser>();
            services.ConfigureOptions(typeof(ProcessesFileConfiguration));
            return services;
        }
    }
}

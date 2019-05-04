using Microsoft.Extensions.DependencyInjection;
using ST.Procesess.Abstraction;
using ST.Procesess.Parsers;
using ST.Process.Razor.Helpers;

namespace ST.Process.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register processes dependencies 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddProcesses(this IServiceCollection services)
        {
            services.AddTransient<IProcessParser, ProcessParser>();
            services.ConfigureOptions(typeof(ProcessesFileConfiguration));
            return services;
        }
    }
}

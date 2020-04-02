using Microsoft.Extensions.DependencyInjection;

namespace GR.Files.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register files razor module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddFilesRazorModule(this IServiceCollection services)
        {
            return services;
        }
    }
}

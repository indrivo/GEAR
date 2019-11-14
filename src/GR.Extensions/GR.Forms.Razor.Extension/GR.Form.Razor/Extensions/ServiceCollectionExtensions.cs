using Microsoft.Extensions.DependencyInjection;
using GR.Forms.Razor.Helpers;

namespace GR.Forms.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register page render
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddFormStaticFilesModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(FormFileConfiguration));
            return services;
        }
    }
}

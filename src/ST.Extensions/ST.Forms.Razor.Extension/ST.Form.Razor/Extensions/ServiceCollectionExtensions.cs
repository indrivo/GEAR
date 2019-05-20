using Microsoft.Extensions.DependencyInjection;
using ST.Forms.Razor.Helpers;

namespace ST.Forms.Razor.Extensions
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

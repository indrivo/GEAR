using GR.Core.Razor.Helpers;
using Microsoft.AspNetCore.Builder;
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
            //services.ConfigureOptions(typeof(CoreRazorFileConfiguration));
            return services;
        }

        /// <summary>
        /// Add configured Cors
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddOriginCorsModule(this IServiceCollection services)
        {
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy", b =>
            //    {
            //        b.AllowAnyOrigin()
            //            .AllowAnyMethod()
            //            .AllowAnyHeader()
            //            .AllowCredentials();
            //    });
            //});

            return services;
        }

        /// <summary>
        /// Use Cors
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConfiguredCors(this IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy")
                .UseStaticFiles()
                .UseSession();
            return app;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using ST.Entities.Security.Abstractions.ServiceBuilder;
using ST.Entities.Security.Razor.Helpers;

namespace ST.Entities.Security.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IEntitySecurityServiceCollection AddEntitySecurityRazorUIModule(this IEntitySecurityServiceCollection services)
        {
            services.Services.ConfigureOptions(typeof(EntitySecurityRazorFileConfiguration));
            return services;
        }
    }
}

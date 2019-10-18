using Microsoft.Extensions.DependencyInjection;
using GR.Entities.Security.Abstractions.ServiceBuilder;
using GR.Entities.Security.Razor.Helpers;

namespace GR.Entities.Security.Razor.Extensions
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

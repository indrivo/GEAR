using Microsoft.Extensions.DependencyInjection;
using GR.Dashboard.Abstractions.ServiceBuilder;
using GR.Dashboard.Razor.Helpers;

namespace GR.Dashboard.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IDashboardServiceCollection AddDashboardRazorUIModule(this IDashboardServiceCollection services)
        {
            services.Services.ConfigureOptions(typeof(DashBoardFileConfiguration));
            return services;
        }
    }
}

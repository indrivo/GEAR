using Microsoft.Extensions.DependencyInjection;
using ST.Dashboard.Abstractions.ServiceBuilder;
using ST.Dashboard.Razor.Helpers;

namespace ST.Dashboard.Razor.Extensions
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

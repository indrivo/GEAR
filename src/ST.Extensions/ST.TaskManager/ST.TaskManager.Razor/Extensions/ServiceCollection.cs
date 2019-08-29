using Microsoft.Extensions.DependencyInjection;
using ST.TaskManager.Razor.Helpers;

namespace ST.TaskManager.Razor.Extensions
{
    public static class ServiceCollection
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTaskManagerRazorUIModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(TaskManagerRazorFileConfiguration));
            return services;
        }
    }
}

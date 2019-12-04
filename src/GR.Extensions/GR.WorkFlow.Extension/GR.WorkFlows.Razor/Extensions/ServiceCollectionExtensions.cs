using GR.WorkFlows.Razor.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace GR.WorkFlows.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add razor ui for workflow module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWorkflowRazorModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(WorkflowRazorFileConfiguration));
            return services;
        }
    }
}

using GR.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace GR.WorkFlows.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddWorkFlowModule<TService>(this IServiceCollection services)
            where TService : class, IWorkFlowService
        {
            IoC.RegisterTransientService<IWorkFlowService, TService>();
            return services;
        }
    }
}

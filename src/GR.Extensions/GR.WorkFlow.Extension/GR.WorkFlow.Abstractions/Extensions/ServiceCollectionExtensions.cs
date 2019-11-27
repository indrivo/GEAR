using GR.Core.Helpers;
using GR.WorkFlows.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GR.WorkFlows.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add workflow module
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWorkFlowModule<TEntity, TService>(this IServiceCollection services)
            where TService : class, IWorkFlowCreatorService<TEntity>
            where TEntity : WorkFlow
        {
            IoC.RegisterTransientService<IWorkFlowCreatorService<TEntity>, TService>();
            return services;
        }
    }
}

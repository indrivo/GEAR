using Microsoft.Extensions.DependencyInjection;
using GR.Audit.Abstractions.Helpers;
using GR.Core.Abstractions;
using GR.Core.Helpers;

namespace GR.Audit.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register audit module
        /// </summary>
        /// <typeparam name="TManager"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuditModule<TManager>(this IServiceCollection services)
            where TManager : class, IAuditManager
        {
            IoC.RegisterTransientService<IAuditManager, TManager>();
            return services;
        }

        /// <summary>
        /// Register module for track audit
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterAuditFor<TContext>(this IServiceCollection services, string moduleName) where TContext : IDbContext
        {
            TrackerContextsInMemory.Register(moduleName, typeof(TContext));
            return services;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Helpers;

namespace ST.Entities.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add entity model
        /// </summary>
        /// <typeparam name="TEntityContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEntityModule<TEntityContext>(this IServiceCollection services) where TEntityContext : DbContext, IEntityContext
        {
            services.AddTransient<IEntityContext, TEntityContext>();
            IoC.RegisterService<IEntityContext, TEntityContext>();
            return services;
        }
    }
}

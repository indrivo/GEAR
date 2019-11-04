using Microsoft.Extensions.DependencyInjection;
using GR.Core.Helpers;
using GR.Orders.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.Orders.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register order services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterProductOrderServices<TOrder, TOrderProductService>(this IServiceCollection services)
            where TOrderProductService : class, IOrderProductService<TOrder>
            where TOrder : Order
        {
            IoC.RegisterTransientService<IOrderProductService<TOrder>, TOrderProductService>();

            return services;
        }

        /// <summary>
        /// Register orders storage
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterOrdersStorage<TContext>(this IServiceCollection services)
            where TContext : DbContext, IOrderDbContext
        {
            services.AddTransient<IOrderDbContext, TContext>();
            IoC.RegisterService<IOrderDbContext>(nameof(IOrderDbContext), typeof(TContext));
            return services;
        }
    }
}

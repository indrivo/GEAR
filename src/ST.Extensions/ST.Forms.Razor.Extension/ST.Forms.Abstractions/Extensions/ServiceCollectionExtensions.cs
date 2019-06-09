using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Helpers;

namespace ST.Forms.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add form module
        /// </summary>
        /// <typeparam name="TFormContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddFormModule<TFormContext>(this IServiceCollection services) where TFormContext : DbContext, IFormContext
        {
            IFormContext ContextFactory(IServiceProvider x)
            {
                var context = x.GetService<TFormContext>();
                IoC.RegisterScopedService<IFormContext, TFormContext>(context);
                return context;
            }

            services.AddScoped(ContextFactory);
            return services;
        }
    }
}

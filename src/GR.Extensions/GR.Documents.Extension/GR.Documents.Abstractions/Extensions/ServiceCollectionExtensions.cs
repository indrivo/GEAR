using GR.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GR.Documents.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register Document type service
        /// </summary>
        /// <typeparam name="TDocumentTypeService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterDocumentTypeServices<TDocumentTypeService>(this IServiceCollection services)
            where TDocumentTypeService : class, IDocumentTypeService
        {
            IoC.RegisterTransientService<IDocumentTypeService, TDocumentTypeService>();

            return services;
        }
        
       /// <summary>
       /// Register documents context
       /// </summary>
       /// <typeparam name="TDocumentContext"></typeparam>
       /// <param name="services"></param>
       /// <param name="storageOptions"></param>
       /// <returns></returns>
        public static IServiceCollection RegisterDocumentStorage<TDocumentContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> storageOptions)
            where TDocumentContext : DbContext, IDocumentContext
        {
            services.AddDbContext<TDocumentContext>(storageOptions);
            IoC.RegisterTransientService<IDocumentContext, TDocumentContext>();
            return services;
        }
    }
}

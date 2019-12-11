using GR.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using Microsoft.AspNetCore.Hosting;

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
        /// Register Document category service
        /// </summary>
        /// <typeparam name="TDocumentTypeService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterDocumentCategoryServices<TDocumentCategoryService>(this IServiceCollection services)
            where TDocumentCategoryService : class, IDocumentCategoryService
        {
            IoC.RegisterTransientService<IDocumentCategoryService, TDocumentCategoryService>();

            return services;
        }

        /// <summary>
        /// Register Document  service
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterDocumentServices<TDocumentService>(this IServiceCollection services)
            where TDocumentService : class, IDocumentService
        {
            IoC.RegisterTransientService<IDocumentService, TDocumentService>();

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
            SystemEvents.Database.OnMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TDocumentContext>();
            };

            return services;
        }
    }
}

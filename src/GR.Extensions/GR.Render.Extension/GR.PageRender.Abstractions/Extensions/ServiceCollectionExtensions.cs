using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GR.Audit.Abstractions.Extensions;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.PageRender.Abstractions.Events;
using Microsoft.AspNetCore.Hosting;

namespace GR.PageRender.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add page module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPageModule(this IServiceCollection services)
        {
            Arg.NotNull(services, nameof(services));
            DynamicUiEvents.RegisterEvents();
            return services;
        }

        /// <summary>
        /// Add module storage
        /// </summary>
        /// <typeparam name="TPageContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddPageModuleStorage<TPageContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
            where TPageContext : DbContext, IDynamicPagesContext
        {
            services.AddScopedContextFactory<IDynamicPagesContext, TPageContext>();
            services.AddDbContext<TPageContext>(options);
            services.RegisterAuditFor<IDynamicPagesContext>("Page module");

            SystemEvents.Database.OnMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TPageContext>();
            };
            return services;
        }
    }
}

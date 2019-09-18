using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Dashboard.Abstractions.Models;
using ST.Dashboard.Abstractions.Models.WidgetTypes;
using ST.Dashboard.Abstractions.ServiceBuilder;

namespace ST.Dashboard.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add dashboard module
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IDashboardServiceCollection AddDashboardModule<TRepository>(this IServiceCollection services)
            where TRepository : class, IDashboardManager
        {
            services.AddTransient<IDashboardManager, TRepository>();
            IoC.RegisterService<IDashboardManager, TRepository>();
            return new DashboardServiceCollection(services);
        }

        /// <summary>
        /// Add dashboard module storage
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IDashboardServiceCollection AddDashboardModuleStorage<TDbContext>(
            this IDashboardServiceCollection configuration, Action<DbContextOptionsBuilder> options)
            where TDbContext : DbContext, IDashboardDbContext
        {
            Arg.NotNull(configuration.Services, nameof(AddDashboardModuleStorage));
            configuration.Services.AddDbContext<TDbContext>(options);
            configuration.Services.AddScopedContextFactory<IDashboardDbContext, TDbContext>();
            return configuration;
        }

        /// <summary>
        /// Register render service
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IDashboardServiceCollection AddDashboardRenderServices(this IDashboardServiceCollection configuration)
        {
            IoC.RegisterServiceCollection(new Dictionary<Type, Type>
            {
                { typeof(IWidgetRenderer<Widget>), typeof(IWidgetRenderer<Widget>)},
                { typeof(IWidgetRenderer<TabbedWidget>), typeof(IWidgetRenderer<TabbedWidget>)}
            });
            return configuration;
        }
    }
}

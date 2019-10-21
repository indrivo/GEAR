using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GR.Core.Extensions;

namespace GR.Report.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add report module
        /// </summary>
        /// <typeparam name="TDynamicReportService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDynamicReportModule<TDynamicReportService>(this IServiceCollection services)
            where TDynamicReportService : class, IDynamicReportsService
        {
            services.AddTransient<IDynamicReportsService, TDynamicReportService>();

            return services;
        }

        /// <summary>
        /// Register module storage
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddDynamicReportModuleStorage<TReportContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
            where TReportContext : DbContext, IReportContext
        {
            services.AddDbContext<TReportContext>(options);
            services.AddScopedContextFactory<IReportContext, TReportContext>();
            return services;
        }
    }
}
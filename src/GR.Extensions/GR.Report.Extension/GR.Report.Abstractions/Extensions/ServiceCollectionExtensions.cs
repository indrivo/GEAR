using System;
using GR.Audit.Abstractions.Extensions;
using GR.Core;
using GR.Core.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GR.Core.Extensions;
using Microsoft.AspNetCore.Hosting;

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
            services.AddGearTransient<IDynamicReportsService, TDynamicReportService>();
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
            services.RegisterAuditFor<IReportContext>($"{nameof(Report)} module");
            SystemEvents.Database.OnMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TReportContext>();
            };
            return services;
        }
    }
}
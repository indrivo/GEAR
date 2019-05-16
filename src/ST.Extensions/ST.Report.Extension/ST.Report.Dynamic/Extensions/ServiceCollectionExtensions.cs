using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Report.Abstractions;

namespace ST.Report.Dynamic.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamicReportModule<TContext>(this IServiceCollection services) where TContext : DbContext, IReportContext
        {
            services.AddTransient<IDynamicReportsService, DynamicReportsService<TContext>>();

            return services;
        }
    }
}

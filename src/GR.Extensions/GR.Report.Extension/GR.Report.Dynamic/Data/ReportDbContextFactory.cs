using Microsoft.EntityFrameworkCore.Design;
using GR.Core.Helpers.DbContexts;

namespace GR.Report.Dynamic.Data
{
    public class ReportDbContextFactory : IDesignTimeDbContextFactory<DynamicReportDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DynamicReportDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<DynamicReportDbContext, DynamicReportDbContext>.CreateFactoryDbContext();
        }
    }
}

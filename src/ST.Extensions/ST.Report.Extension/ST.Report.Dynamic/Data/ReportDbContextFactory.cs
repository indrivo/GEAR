using Microsoft.EntityFrameworkCore.Design;
using ST.Core.Helpers.DbContexts;

namespace ST.Report.Dynamic.Data
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

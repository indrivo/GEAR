using Microsoft.EntityFrameworkCore;
using GR.Core.Abstractions;
using GR.Report.Abstractions;
using GR.Report.Abstractions.Models;

namespace GR.Report.Dynamic.Data
{
    public class DynamicReportDbContext : DbContext, IReportContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Report";

        public DynamicReportDbContext(DbContextOptions<DynamicReportDbContext> options) : base(options)
        {

        }

        /// <summary>
        /// Dynamic reports
        /// </summary>
        public DbSet<DynamicReport> DynamicReports { get; set; }

        /// <summary>
        /// Dynamic report folders
        /// </summary>
        public DbSet<DynamicReportFolder> DynamicReportsFolders { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
        }

        public DbSet<T> SetEntity<T>() where T : class, IBaseModel
        {
            return Set<T>();
        }
    }
}

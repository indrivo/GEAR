using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ST.Report.Abstractions;
using ST.Report.Abstractions.Models;

namespace ST.Report.Dynamic.Data
{
    public class DynamicReportDbContext : DbContext, IReportContext
    {
        public DynamicReportDbContext(DbContextOptions options) : base(options)
        {

        }

        /// <summary>
        /// Dynamic reports
        /// </summary>
        public DbSet<DynamicReportDbModel> DynamicReports { get; set; }

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
        }
    }

    public class DynamicReportDbContextContextFactory : IDesignTimeDbContextFactory<DynamicReportDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DynamicReportDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DynamicReportDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=1111;Database=ISODMS.DEV;");
            return new DynamicReportDbContext(optionsBuilder.Options);
        }
    }
}

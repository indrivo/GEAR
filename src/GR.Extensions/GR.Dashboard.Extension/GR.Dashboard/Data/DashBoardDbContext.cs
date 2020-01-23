using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Audit.Contexts;
using GR.Core.Helpers;
using GR.Dashboard.Abstractions;
using GR.Dashboard.Abstractions.Models;
using GR.Dashboard.Abstractions.Models.Permissions;
using GR.Dashboard.Abstractions.Models.RowWidgets;
using GR.Dashboard.Abstractions.Models.WidgetTypes;

namespace GR.Dashboard.Data
{
    public class DashBoardDbContext : TrackerDbContext, IDashboardDbContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this, is used on audit 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public const string Schema = "DashBoard";

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public DashBoardDbContext(DbContextOptions<DashBoardDbContext> options) : base(options)
        {
        }

        #region Entities

        public virtual DbSet<WidgetGroup> WidgetGroups { get; set; }
        public virtual DbSet<DashBoard> Dashboards { get; set; }
        public virtual DbSet<Row> Rows { get; set; }
        public virtual DbSet<CustomWidget> CustomWidgets { get; set; }
        public virtual DbSet<ChartWidget> ChartWidgets { get; set; }
        public virtual DbSet<ListWidget> ListWidgets { get; set; }
        public virtual DbSet<ReportWidget> ReportWidgets { get; set; }
        public virtual DbSet<TabbedWidget> TabbedWidgets { get; set; }
        public virtual DbSet<RowChartWidget> RowChartWidgets { get; set; }
        public virtual DbSet<RowCustomWidget> RowCustomWidgets { get; set; }
        public virtual DbSet<RowReportWidget> RowReportWidgets { get; set; }
        public virtual DbSet<RowWidgetAcl> WidgetAcls { get; set; }

        #endregion

        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
            var path = Path.Combine(AppContext.BaseDirectory, "Configuration/WidgetGroups.json");
            var items = JsonParser.ReadArrayDataFromJsonFile<ICollection<WidgetGroup>>(path);
            builder.Entity<WidgetGroup>().HasData(items);
            builder.Entity<RowReportWidget>().HasKey(x => new { x.ReportWidgetId, x.RowId });
            builder.Entity<RowChartWidget>().HasKey(x => new { x.ChartWidgetId, x.RowId });
            builder.Entity<RowCustomWidget>().HasKey(x => new { x.CustomWidgetId, x.RowId });
            builder.Entity<RowWidgetAcl>().HasKey(x => new { x.RowId, x.WidgetId, x.RoleId });
        }

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            var service = IoC.Resolve<IDashboardService>();
            service?.SeedWidgetsAsync().Wait();

            return Task.CompletedTask;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using GR.Audit.Contexts;
using GR.Core.Abstractions;
using GR.Core.Helpers;
using GR.Dashboard.Abstractions;
using GR.Dashboard.Abstractions.Models;
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

        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
            var path = Path.Combine(AppContext.BaseDirectory, "Configurations\\WidgetGroups.json");
            var items = JsonParser.ReadArrayDataFromJsonFile<ICollection<WidgetGroup>>(path);
            builder.Entity<WidgetGroup>().HasData(items);
            builder.Entity<RowReportWidget>().HasKey(x => new { x.ReportWidgetId, x.RowId });
            builder.Entity<RowChartWidget>().HasKey(x => new { x.ChartWidgetId, x.RowId });
            builder.Entity<RowCustomWidget>().HasKey(x => new { x.CustomWidgetId, x.RowId });
        }

        /// <inheritdoc />
        /// <summary>
        /// Set entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual DbSet<T> SetEntity<T>() where T : class, IBaseModel => Set<T>();

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
    }
}

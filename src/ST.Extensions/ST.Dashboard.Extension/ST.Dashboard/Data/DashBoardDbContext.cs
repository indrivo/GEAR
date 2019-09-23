using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using ST.Audit.Contexts;
using ST.Core.Abstractions;
using ST.Core.Helpers;
using ST.Dashboard.Abstractions;
using ST.Dashboard.Abstractions.Models;
using ST.Dashboard.Abstractions.Models.WidgetTypes;

namespace ST.Dashboard.Data
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
        }

        /// <inheritdoc />
        /// <summary>
        /// Set entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual DbSet<T> SetEntity<T>() where T : class, IBaseModel
        {
            return Set<T>();
        }

        public virtual DbSet<WidgetGroup> WidgetGroups { get; set; }
        public DbSet<DashBoard> Dashboards { get; set; }
        public virtual DbSet<Row> Rows { get; set; }
        public virtual DbSet<CustomWidget> CustomWidgets { get; set; }
        public virtual DbSet<ChartWidget> ChartWidgets { get; set; }
        public virtual DbSet<ListWidget> ListWidgets { get; set; }
        public virtual DbSet<ReportWidget> ReportWidgets { get; set; }
        public virtual DbSet<TabbedWidget> TabbedWidgets { get; set; }
    }
}

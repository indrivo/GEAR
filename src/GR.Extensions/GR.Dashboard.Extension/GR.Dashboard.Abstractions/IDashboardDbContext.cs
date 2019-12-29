using Microsoft.EntityFrameworkCore;
using GR.Core.Abstractions;
using GR.Dashboard.Abstractions.Models;
using GR.Dashboard.Abstractions.Models.Permissions;
using GR.Dashboard.Abstractions.Models.RowWidgets;
using GR.Dashboard.Abstractions.Models.WidgetTypes;

namespace GR.Dashboard.Abstractions
{
    public interface IDashboardDbContext : IDbContext
    {
        /// <summary>
        /// Widget groups
        /// </summary>
        DbSet<WidgetGroup> WidgetGroups { get; set; }

        /// <summary>
        /// Dashboards
        /// </summary>
        DbSet<DashBoard> Dashboards { get; set; }

        /// <summary>
        /// Rows
        /// </summary>
        DbSet<Row> Rows { get; set; }

        /// <summary>
        /// Sample widgets
        /// </summary>
        DbSet<CustomWidget> CustomWidgets { get; set; }

        /// <summary>
        /// Chart widgets
        /// </summary>
        DbSet<ChartWidget> ChartWidgets { get; set; }

        /// <summary>
        /// List widgets
        /// </summary>
        DbSet<ListWidget> ListWidgets { get; set; }

        /// <summary>
        /// Report widgets
        /// </summary>
        DbSet<ReportWidget> ReportWidgets { get; set; }

        /// <summary>
        /// Tabbed widgets
        /// </summary>
        DbSet<TabbedWidget> TabbedWidgets { get; set; }

        /// <summary>
        /// Widgets
        /// </summary>
        DbSet<RowChartWidget> RowChartWidgets { get; set; }

        /// <summary>
        /// Widgets
        /// </summary>
        DbSet<RowCustomWidget> RowCustomWidgets { get; set; }

        /// <summary>
        /// Widgets
        /// </summary>
        DbSet<RowReportWidget> RowReportWidgets { get; set; }

        /// <summary>
        /// Access to view
        /// </summary>
        DbSet<RowWidgetAcl> WidgetAcls { get; set; }
    }
}
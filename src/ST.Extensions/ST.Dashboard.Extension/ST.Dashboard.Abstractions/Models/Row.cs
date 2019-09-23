using System;
using System.Collections.Generic;
using ST.Core;
using ST.Dashboard.Abstractions.Models.WidgetTypes;

namespace ST.Dashboard.Abstractions.Models
{
    public class Row : BaseModel
    {
        public string Name { get; set; }

        /// <summary>
        /// Row order
        /// </summary>
        public virtual int Order { get; set; } = 1;

        /// <summary>
        /// Ref to dashboard
        /// </summary>
        public virtual DashBoard DashBoard { get; set; }
        public virtual Guid DashboardId { get; set; }

        /// <summary>
        /// Sample Widgets
        /// </summary>
        public virtual ICollection<Widget> Widgets { get; set; } = new List<Widget>();

        /// <summary>
        /// Chart Widgets
        /// </summary>
        public virtual ICollection<ChartWidget> ChartWidgets { get; set; } = new List<ChartWidget>();

        /// <summary>
        /// List widgets
        /// </summary>
        public virtual ICollection<ListWidget> ListWidgets { get; set; } = new List<ListWidget>();

        /// <summary>
        /// Report widgets
        /// </summary>
        public virtual ICollection<ReportWidget> ReportWidgets { get; set; } = new List<ReportWidget>();

        /// <summary>
        /// Tabbed widgets
        /// </summary>
        public virtual ICollection<TabbedWidget> TabbedWidgets { get; set; } = new List<TabbedWidget>();
    }
}

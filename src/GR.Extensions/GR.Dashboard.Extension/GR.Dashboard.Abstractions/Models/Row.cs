using System;
using System.Collections.Generic;
using GR.Core;
using GR.Dashboard.Abstractions.Models.RowWidgets;

namespace GR.Dashboard.Abstractions.Models
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
        /// Get widget html bodies
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(Widget, string)> GetWidgetBodies()
        {
            var widgets = new (Widget, string)[CustomWidgets.Count + ChartWidgets.Count + ReportWidgets.Count];
            foreach (var widget in CustomWidgets)
            {
                widgets[widget.Order] = (widget.CustomWidget, widget.CustomWidget.Render());
            }

            foreach (var widget in ChartWidgets)
            {
                widgets[widget.Order] = (widget.ChartWidget, widget.ChartWidget.Render());
            }

            foreach (var widget in ReportWidgets)
            {
                widgets[widget.Order] = (widget.ReportWidget, widget.ReportWidget.Render());
            }

            return widgets;
        }

        /// <summary>
        /// Sample Widgets
        /// </summary>
        public virtual ICollection<RowCustomWidget> CustomWidgets { get; set; } = new List<RowCustomWidget>();

        /// <summary>
        /// Chart Widgets
        /// </summary>
        public virtual ICollection<RowChartWidget> ChartWidgets { get; set; } = new List<RowChartWidget>();

        /// <summary>
        /// Report widgets
        /// </summary>
        public virtual ICollection<RowReportWidget> ReportWidgets { get; set; } = new List<RowReportWidget>();
    }
}

using System;
using GR.Dashboard.Abstractions.Models.WidgetTypes;

namespace GR.Dashboard.Abstractions.Models.RowWidgets
{
    public sealed class RowChartWidget : WidgetUISettings
    {
        /// <summary>
        /// Row ref
        /// </summary>
        public Row Row { get; set; }
        public Guid RowId { get; set; }

        /// <summary>
        /// Report widget id
        /// </summary>
        public ChartWidget ChartWidget { get; set; }
        public Guid ChartWidgetId { get; set; }

        /// <summary>
        /// Order
        /// </summary>
        public int Order { get; set; } = 1;
    }
}

using System;
using GR.Dashboard.Abstractions.Models.WidgetTypes;

namespace GR.Dashboard.Abstractions.Models.RowWidgets
{
    public sealed class RowReportWidget : WidgetUISettings
    {
        /// <summary>
        /// Row ref
        /// </summary>
        public Row Row { get; set; }
        public Guid RowId { get; set; }

        /// <summary>
        /// Report widget id
        /// </summary>
        public ReportWidget ReportWidget { get; set; }
        public Guid ReportWidgetId { get; set; }

        /// <summary>
        /// Order
        /// </summary>
        public int Order { get; set; } = 1;
    }
}

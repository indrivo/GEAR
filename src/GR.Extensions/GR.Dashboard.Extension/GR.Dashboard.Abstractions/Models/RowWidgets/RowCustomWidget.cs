using System;
using GR.Dashboard.Abstractions.Models.WidgetTypes;

namespace GR.Dashboard.Abstractions.Models.RowWidgets
{
    public class RowCustomWidget : WidgetUISettings
    {
        /// <summary>
        /// Row ref
        /// </summary>
        public Row Row { get; set; }
        public Guid RowId { get; set; }

        /// <summary>
        /// Report widget id
        /// </summary>
        public CustomWidget CustomWidget { get; set; }
        public Guid CustomWidgetId { get; set; }

        /// <summary>
        /// Order
        /// </summary>
        public int Order { get; set; } = 1;
    }
}

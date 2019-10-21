using System.Collections.Generic;
using GR.Dashboard.Abstractions.Models.RowWidgets;

namespace GR.Dashboard.Abstractions.Models.WidgetTypes
{
    public class ChartWidget : Widget
    {
        /// <summary>
        /// Row reference
        /// </summary>
        public virtual ICollection<RowChartWidget> ChartWidgets { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Render
        /// </summary>
        /// <returns></returns>
        public override string Render()
        {
            return Service<ChartWidget>().Render(this);
        }
    }
}

using System;

namespace ST.Dashboard.Abstractions.Models.WidgetTypes
{
    public class ChartWidget : Widget
    {
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

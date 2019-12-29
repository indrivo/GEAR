using System.Collections.Generic;
using GR.Dashboard.Abstractions.Models.RowWidgets;

namespace GR.Dashboard.Abstractions.Models.WidgetTypes
{
    public class CustomWidget : Widget
    {
        /// <summary>
        /// Row ref
        /// </summary>
        public virtual ICollection<RowCustomWidget> CustomWidgets { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Render
        /// </summary>
        /// <returns></returns>
        public override string Render()
        {
            return Service<CustomWidget>().Render(this);
        }
    }
}

using System;
using System.Collections.Generic;

namespace GR.Dashboard.Abstractions.Models.WidgetTypes
{
    public class TabbedWidget : Widget
    {
        /// <summary>
        /// Tabs
        /// </summary>
        public ICollection<WidgetTab> Tabs { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Render tabs
        /// </summary>
        /// <returns></returns>
        public override string Render()
        {
            return Service<TabbedWidget>().Render(this);
        }
    }

    public class WidgetTab : Widget
    {
        /// <summary>
        /// Widget tab reference
        /// </summary>
        public TabbedWidget TabbedWidget { get; set; }
        public Guid TabbedWidgetId { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Render
        /// </summary>
        /// <returns></returns>
        public override string Render()
        {
            return Service<WidgetTab>().Render(this);
        }
    }
}

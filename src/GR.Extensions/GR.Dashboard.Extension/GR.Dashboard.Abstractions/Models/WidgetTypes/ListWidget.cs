using System;
using System.Collections.Generic;
using System.Text;

namespace GR.Dashboard.Abstractions.Models.WidgetTypes
{
    public class ListWidget : Widget
    {
        public override string Render()
        {
            return Service<ListWidget>().Render(this);
        }
    }
}

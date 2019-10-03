using System.Collections.Generic;
using ST.Dashboard.Abstractions.Models.WidgetTypes;

namespace ST.Dashboard.Abstractions.Models.ViewModels
{
    public class WidgetViewModel : CustomWidget
    {
        public IEnumerable<WidgetGroup> Groups { get; set; } = new List<WidgetGroup>();
    }
}

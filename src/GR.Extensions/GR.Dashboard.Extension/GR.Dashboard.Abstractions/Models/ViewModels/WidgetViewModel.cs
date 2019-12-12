using System.Collections.Generic;
using GR.Dashboard.Abstractions.Models.WidgetTypes;

namespace GR.Dashboard.Abstractions.Models.ViewModels
{
    public class WidgetViewModel : CustomWidget
    {
        public IEnumerable<WidgetGroup> Groups { get; set; } = new List<WidgetGroup>();
    }
}

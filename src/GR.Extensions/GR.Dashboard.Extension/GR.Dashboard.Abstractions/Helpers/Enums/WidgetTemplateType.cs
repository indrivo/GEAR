using System.ComponentModel;

namespace GR.Dashboard.Abstractions.Helpers.Enums
{
    public enum WidgetTemplateType
    {
        [Description("Sample template")]
        Sample = 1001,
        [Description("Html template")]
        Html = 1002,
        [Description(".Net razor template")]
        Razor = 1003
    }
}
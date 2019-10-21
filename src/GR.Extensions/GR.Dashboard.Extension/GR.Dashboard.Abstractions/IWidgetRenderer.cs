using GR.Dashboard.Abstractions.Models;

namespace GR.Dashboard.Abstractions
{
    public interface IWidgetRenderer<in TWidget> where TWidget : Widget
    {
        /// <summary>
        /// Render widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        string Render(TWidget widget);
    }
}
using ST.Dashboard.Abstractions.Models;

namespace ST.Dashboard.Abstractions
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
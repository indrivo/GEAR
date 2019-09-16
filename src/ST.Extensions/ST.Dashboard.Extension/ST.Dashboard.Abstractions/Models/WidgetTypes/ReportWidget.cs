namespace ST.Dashboard.Abstractions.Models.WidgetTypes
{
    public class ReportWidget : Widget
    {
        /// <inheritdoc />
        /// <summary>
        /// Render
        /// </summary>
        /// <returns></returns>
        public override string Render()
        {
            return Service<ReportWidget>().Render(this);
        }
    }
}

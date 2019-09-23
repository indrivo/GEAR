namespace ST.Dashboard.Abstractions.Models
{
    public interface IWidgetUISettings
    {
        /// <summary>
        /// Width
        /// </summary>
        string Width { get; set; }
        /// <summary>
        /// Height
        /// </summary>
        string Height { get; set; }

        /// <summary>
        /// Color
        /// </summary>
        string BackGroundColor { get; set; }

        /// <summary>
        /// Border radius
        /// </summary>
        int BorderRadius { get; set; }

        /// <summary>
        /// Border style
        /// </summary>
        string BorderStyle { get; set; }

        /// <summary>
        /// Css class
        /// </summary>
        string ClassAttribute { get; set; }
    }
}

namespace GR.Dashboard.Abstractions.Models
{
    public class WidgetUISettings
    {
        /// <summary>
        /// Width
        /// </summary>
        public virtual string Width { get; set; }
        /// <summary>
        /// Height
        /// </summary>
        public virtual string Height { get; set; }

        /// <summary>
        /// Color
        /// </summary>
        public virtual string BackGroundColor { get; set; }

        /// <summary>
        /// Border radius
        /// </summary>
        public virtual int BorderRadius { get; set; }

        /// <summary>
        /// Border style
        /// </summary>
        public virtual string BorderStyle { get; set; }

        /// <summary>
        /// Css class
        /// </summary>
        public virtual string ClassAttribute { get; set; }
    }
}

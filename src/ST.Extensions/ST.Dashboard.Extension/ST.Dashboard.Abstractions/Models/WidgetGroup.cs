using System.ComponentModel.DataAnnotations;
using ST.Core;

namespace ST.Dashboard.Abstractions.Models
{
    public class WidgetGroup : BaseModel
    {
        /// <summary>
        /// Group name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }
    }
}

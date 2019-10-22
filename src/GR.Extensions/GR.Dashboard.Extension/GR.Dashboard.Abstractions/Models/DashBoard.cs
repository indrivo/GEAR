using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.Dashboard.Abstractions.Models
{
    public class DashBoard : BaseModel
    {
        /// <summary>
        /// Dashboard name 
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Dashboard rows
        /// </summary>
        public virtual ICollection<Row> Rows { get; set; }

        /// <summary>
        /// Is active
        /// </summary>
        public virtual bool IsActive { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.ECommerce.Abstractions.Models
{
    public class Brand : BaseModel
    {
        /// <summary>
        /// Brand name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public virtual string DisplayName { get; set; }
    }
}

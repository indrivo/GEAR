using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.ECommerce.Abstractions.Models
{
    public class AttributeGroup : BaseModel
    {
        /// <summary>
        /// Attribute group name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }
    }
}

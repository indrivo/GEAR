using System.ComponentModel.DataAnnotations;
using ST.Core;

namespace ST.ECommerce.Abstractions.Models
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

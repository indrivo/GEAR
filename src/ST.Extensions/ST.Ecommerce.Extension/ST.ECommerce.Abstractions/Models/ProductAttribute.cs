using System;
using System.ComponentModel.DataAnnotations;
using ST.Core;

namespace ST.ECommerce.Abstractions.Models
{
    public class ProductAttribute : BaseModel
    {
        /// <summary>
        /// Attribute name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        [Required]
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Reference to group
        /// </summary>
        public virtual AttributeGroup AttributeGroup { get; set; }

        public virtual Guid? AttributeGroupId { get; set; }
    }
}
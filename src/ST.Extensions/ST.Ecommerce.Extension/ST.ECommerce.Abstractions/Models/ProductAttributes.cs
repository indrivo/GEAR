using System;
using System.ComponentModel.DataAnnotations;

namespace ST.ECommerce.Abstractions.Models
{
    public class ProductAttributes
    {
        /// <summary>
        /// Reference to product
        /// </summary>
        public virtual Product Product { get; set; }

        [Required] public virtual Guid ProductId { get; set; }

        /// <summary>
        /// Reference to product attribute 
        /// </summary>
        public virtual ProductAttribute ProductAttribute { get; set; }

        [Required] public virtual Guid ProductAttributeId { get; set; }

        /// <summary>
        /// Value Of A Attribute
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Is Attribute Published
        /// </summary>
        public virtual bool IsPublished { get; set; }

        /// <summary>
        /// Is Attribute Available
        /// </summary>
        public virtual bool IsAvailable { get; set; } = true;
    }
}
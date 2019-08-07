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
        [Required]
        public virtual Guid ProductId { get; set; }

        /// <summary>
        /// Reference to product attribute 
        /// </summary>
        public virtual ProductAttribute ProductAttribute { get; set; }
        [Required]
        public virtual Guid ProductAttributeId { get; set; }
    }
}

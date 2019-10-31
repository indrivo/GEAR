using System;
using System.ComponentModel.DataAnnotations;

namespace GR.ECommerce.Abstractions.Models
{
    public class ProductOrder
    {
        /// <summary>
        /// Reference to product
        /// </summary>
        public virtual Product Product { get; set; }
        [Required]
        public virtual Guid ProductId { get; set; }

        /// <summary>
        /// Reference to order 
        /// </summary>
        public virtual Order Order { get; set; }
        [Required]
        public virtual Guid OrderId { get; set; }

        /// <summary>
        /// Product variation ref
        /// </summary>
        public  ProductVariation ProductVariation { get; set; }
        public Guid? ProductVariationId { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        [Required]
        public virtual double Price { get; set; } = 0;
    }
}

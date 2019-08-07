using System;
using System.ComponentModel.DataAnnotations;

namespace ST.ECommerce.Abstractions.Models
{
    public class ProductCategory
    {
        /// <summary>
        /// Reference to product
        /// </summary>
        public virtual Product Product { get; set; }
        [Required]
        public virtual Guid ProductId { get; set; }

        /// <summary>
        /// Reference to category
        /// </summary>
        public virtual Category Category { get; set; }
        [Required]
        public virtual Guid CategoryId { get; set; }

    }
}

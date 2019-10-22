using System;
using System.ComponentModel.DataAnnotations;

namespace GR.ECommerce.Abstractions.Models
{
    public class ProductDiscount
    {
        /// <summary>
        /// Product discount
        /// </summary>
        public virtual Product Product { get; set; }
        public virtual Guid ProductId { get; set; }

        /// <summary>
        /// Discount
        /// </summary>
        public Discount Discount { get; set; }
        public virtual Guid DiscountId { get; set; }
    }
}

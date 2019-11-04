using System;
using System.ComponentModel.DataAnnotations;
using GR.ECommerce.Abstractions.Models;

namespace GR.Orders.Abstractions.Models
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
        public ProductVariation ProductVariation { get; set; }
        public Guid? ProductVariationId { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        [Required]
        public virtual decimal PriceWithOutDiscount { get; set; } = 0;

        /// <summary>
        /// Get set
        /// </summary>
        public virtual decimal FinalPrice => PriceWithOutDiscount - DiscountValue;

        /// <summary>
        /// Final price for all 
        /// </summary>
        public virtual decimal AmountFinalPrice => FinalPrice * Amount;

        /// <summary>
        /// Amount price without discount
        /// </summary>
        public virtual decimal AmountFinalPriceWithOutDiscount => PriceWithOutDiscount * Amount;

        /// <summary>
        /// Discount value
        /// </summary>
        public virtual decimal DiscountValue { get; set; } = 0;

        /// <summary>
        /// Amount
        /// </summary>
        public virtual int Amount { get; set; } = 0;
    }
}

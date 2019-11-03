using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GR.Core;
using GR.ECommerce.Abstractions.Enums;

namespace GR.Orders.Abstractions.Models
{
    public class Order : BaseModel
    {
        /// <summary>
        /// Reference to customer
        /// </summary>
        [Required]
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// List of ordered products
        /// </summary>
        public virtual IEnumerable<ProductOrder> ProductOrders { get; set; }

        /// <summary>
        /// Quantity
        /// </summary>
        public virtual int Quantity => ProductOrders?.Count() ?? 0;

        /// <summary>
        /// Total
        /// </summary>
        public virtual decimal Total => ProductOrders?.Sum(x => x.FinalPrice * x.Amount) ?? 0;

        /// <summary>
        /// Sub total
        /// </summary>
        public virtual decimal SubTotal => ProductOrders?.Sum(x => x.PriceWithOutDiscount * x.Amount) ?? 0;

        /// <summary>
        /// Discount total
        /// </summary>
        public virtual decimal DiscountTotal => ProductOrders?.Sum(x => x.DiscountValue * x.Amount) ?? 0;

        /// <summary>
        /// Order state
        /// </summary>
        [Required]
        public virtual OrderState OrderState { get; set; } = OrderState.New;

        /// <summary>
        /// Order histories
        /// </summary>
        public virtual IEnumerable<OrderHistory> OrderHistories { get; set; }

        /// <summary>
        /// User notes
        /// </summary>
        [MaxLength(255)]
        public virtual string Notes { get; set; }

        /// <summary>
        /// Billing address
        /// </summary>
        public virtual Guid? BillingAddress { get; set; }

        /// <summary>
        /// Shipment address
        /// </summary>
        public virtual Guid? ShipmentAddress { get; set; }
    }

    public class OrderHistory : BaseModel
    {
        /// <summary>
        /// Order reference
        /// </summary>
        public virtual Order Order { get; set; }
        public virtual Guid OrderId { get; set; }

        /// <summary>
        /// On state changed description  
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Order state
        /// </summary>
        public virtual OrderState OrderState { get; set; } = OrderState.New;
    }
}

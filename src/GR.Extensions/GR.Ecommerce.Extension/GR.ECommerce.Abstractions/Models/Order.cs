using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GR.Core;
using GR.ECommerce.Abstractions.Enums;

namespace GR.ECommerce.Abstractions.Models
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
        public virtual double Total => ProductOrders?.Sum(x => x.Price) ?? 0;

        /// <summary>
        /// Order state
        /// </summary>
        public virtual OrderState OrderState { get; set; } = OrderState.New;

        /// <summary>
        /// Order histories
        /// </summary>
        public virtual IEnumerable<OrderHistory> OrderHistories { get; set; }
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

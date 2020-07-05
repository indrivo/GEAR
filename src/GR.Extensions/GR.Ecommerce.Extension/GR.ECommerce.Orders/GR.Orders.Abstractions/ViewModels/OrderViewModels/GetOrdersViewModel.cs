using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GR.Core;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Abstractions.Models.Currencies;
using GR.Identity.Abstractions.ViewModels.UserViewModels;
using GR.Orders.Abstractions.Models;

namespace GR.Orders.Abstractions.ViewModels.OrderViewModels
{
    public class GetOrdersViewModel : BaseModel
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
        /// Status
        /// </summary>
        public string State => OrderState.ToString();

        /// <summary>
        /// Text
        /// </summary>
        public string FormattedTotal => Currency?.Symbol + Total.ToString("N2");

        /// <summary>
        /// User
        /// </summary>
        public UserInfoViewModel User { get; set; }

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

        /// <summary>
        /// Currency reference
        /// </summary>
        public virtual Currency Currency { get; set; }

        [Required]
        public virtual string CurrencyId { get; set; }
    }
}

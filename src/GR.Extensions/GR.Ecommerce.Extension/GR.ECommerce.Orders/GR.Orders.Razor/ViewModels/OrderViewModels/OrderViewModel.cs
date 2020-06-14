using System.Collections.Generic;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.Identity.Profile.Abstractions.Models.AddressModels;
using GR.Orders.Abstractions.Models;

namespace GR.Orders.Razor.ViewModels.OrderViewModels
{
    public class OrderViewModel
    {
        /// <summary>
        /// Order
        /// </summary>
        public virtual Order Order { get; set; }

        /// <summary>
        /// Payments
        /// </summary>
        public virtual IEnumerable<Payment> Payments { get; set; }

        /// <summary>
        /// Billing address
        /// </summary>
        public virtual Address BillingAddress { get; set; }

        /// <summary>
        /// Shipping address
        /// </summary>
        public virtual Address ShippingAddress { get; set; }
    }
}

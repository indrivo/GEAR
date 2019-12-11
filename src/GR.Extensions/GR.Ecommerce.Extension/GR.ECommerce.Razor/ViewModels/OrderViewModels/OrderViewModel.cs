using System.Collections.Generic;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.Orders.Abstractions.Models;

namespace GR.ECommerce.Razor.ViewModels.OrderViewModels
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
    }
}

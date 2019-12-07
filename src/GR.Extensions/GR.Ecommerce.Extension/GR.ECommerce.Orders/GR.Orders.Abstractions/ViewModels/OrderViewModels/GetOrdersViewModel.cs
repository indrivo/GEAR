using GR.Identity.Abstractions;
using GR.Orders.Abstractions.Models;

namespace GR.Orders.Abstractions.ViewModels.OrderViewModels
{
    public sealed class GetOrdersViewModel : Order
    {
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
        public GearUser User { get; set; }
    }
}

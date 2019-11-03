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
        public string FormattedTotal => "$" + Total.ToString("N2");
    }
}

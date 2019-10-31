using GR.ECommerce.Abstractions.Models;

namespace GR.ECommerce.Abstractions.ViewModels.OrderViewModels
{
    public sealed class GetOrdersViewModel : Order
    {
        /// <summary>
        /// Status
        /// </summary>
        public string State => OrderState.ToString();
    }
}

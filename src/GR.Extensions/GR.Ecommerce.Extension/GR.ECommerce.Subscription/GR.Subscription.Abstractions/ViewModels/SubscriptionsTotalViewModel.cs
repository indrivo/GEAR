using GR.ECommerce.Abstractions.Models.Currencies;

namespace GR.Subscriptions.Abstractions.ViewModels
{
    public class SubscriptionsTotalViewModel
    {
        /// <summary>
        /// Total money
        /// </summary>
        public virtual decimal Total { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public virtual Currency Currency { get; set; }
    }
}

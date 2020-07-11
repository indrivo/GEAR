using System;
using GR.ECommerce.Abstractions.Models.Currencies;
using GR.Identity.Abstractions.ViewModels.UserViewModels;

namespace GR.Subscriptions.Abstractions.ViewModels
{
    public class SubscriptionUserInfoViewModel : UserInfoViewModel
    {
        /// <summary>
        /// Period for paid subscription
        /// </summary>
        public virtual string Period { get; set; }

        /// <summary>
        /// The amount paid for subscription
        /// </summary>
        public virtual decimal Amount { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public virtual Currency Currency { get; set; }

        /// <summary>
        /// The date then subscription has been paid
        /// </summary>
        public virtual DateTime? DatePaid { get; set; }

        /// <summary>
        /// Date then subscription will end
        /// </summary>
        public virtual DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// True if subscription can expire
        /// </summary>
        public virtual bool CanExpire { get; set; }

        /// <summary>
        /// Subscription state
        /// </summary>
        public virtual string Status { get; set; }
    }
}

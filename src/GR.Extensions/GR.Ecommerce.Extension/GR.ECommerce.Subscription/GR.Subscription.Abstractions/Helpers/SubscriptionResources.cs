using System;
using System.Collections.Generic;

namespace GR.Subscriptions.Abstractions.Helpers
{
    public static class SubscriptionResources
    {
        /// <summary>
        /// Subscription plan
        /// </summary>
        public static Guid SubscriptionPlanProductType = Guid.Parse("b9bbcd98-9548-4ff2-9536-22eab2c0218d");

        /// <summary>
        /// Default subscription plan
        /// </summary>
        public static Guid DefaultSubscriptionPlan = Guid.Parse("2e6ce2e7-1188-434b-b361-1a9f1b590e87");

        /// <summary>
        /// Subscription brand
        /// </summary>
        public static Guid SubscriptionBrand = Guid.Parse("29fe5b55-838d-4215-b2a5-28f49ab2e484");
    }

    public class SubscriptionConfiguration
    {
        /// <summary>
        /// Delete on upgrade
        /// </summary>
        public virtual bool DeleteOtherSubscriptionsOnUpgrade { get; set; } = false;

        /// <summary>
        /// Auto creation of default subscription
        /// </summary>
        public virtual bool MissingSubscriptionCreateFreeDefault { get; set; } = true;

        /// <summary>
        /// Notification providers
        /// </summary>
        public virtual IEnumerable<string> NotificationProviders { get; set; } = new List<string>
        {
            "email", "notification.local"
        };
    }
}
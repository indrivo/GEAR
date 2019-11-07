using System;

namespace GR.ECommerce.Abstractions.Helpers
{
    public struct ProductTypes
    {
        /// <summary>
        /// Subscription plan
        /// </summary>
        public static Guid SubscriptionPlan = Guid.Parse("b9bbcd98-9548-4ff2-9536-22eab2c0218d");

        /// <summary>
        /// Others
        /// </summary>
        public static Guid Other = Guid.Empty;
    }
}

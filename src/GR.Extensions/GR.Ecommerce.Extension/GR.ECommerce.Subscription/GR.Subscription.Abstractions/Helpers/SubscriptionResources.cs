using System;
using GR.Identity.Abstractions;
using GR.Subscriptions.Abstractions.Models;

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

        /// <summary>
        /// Configuration
        /// </summary>
        public static SubscriptionConfiguration Configuration { get; internal set; } = new SubscriptionConfiguration();

        public static class QueryResolvers
        {
            /// <summary>
            /// Load only user subscriptions
            /// </summary>
            public static Func<Subscription, GearUser, bool> UserOwnResolver => (subscription, user)
                => subscription.UserId.Equals(user.Id);

            /// <summary>
            /// Load tenant subscription
            /// </summary>
            public static Func<Subscription, GearUser, bool> TenantOwnResolver
                => (subscription, user) => subscription.TenantId.Equals(user.TenantId);
        }
    }

    public class SubscriptionConfiguration
    {
        /// <summary>
        /// Subscription query
        /// </summary>
        public virtual Func<Subscription, GearUser, bool> UserSubscriptionQuery { get; set; } =
            SubscriptionResources.QueryResolvers.UserOwnResolver;

        /// <summary>
        /// Delete on upgrade
        /// </summary>
        public virtual bool DeleteOtherSubscriptionsOnUpgrade { get; set; } = false;
    }
}
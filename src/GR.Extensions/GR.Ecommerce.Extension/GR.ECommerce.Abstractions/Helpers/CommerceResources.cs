using System;

namespace GR.ECommerce.Abstractions.Helpers
{
    public static class CommerceResources
    {
        /// <summary>
        /// Default product type 
        /// </summary>
        public static Guid DefaultProductType = Guid.Parse("ba0a1d29-22ac-4429-ab56-12391b76e7a4");

        public static class SystemCurrencies
        {
            public const string MDL = "MDL";
            public const string USD = "USD";
            public const string EUR = "EUR";
        }

        public static class SettingsParameters
        {
            public const string CURRENCY = "CURRENCY";
            public const string DAYS_NOTIFY_SUBSCRIPTION_EXPIRATION = "DAYS_NOTIFY_SUBSCRIPTION_EXPIRATION";
            public const string FREE_TRIAL_PERIOD_DAYS = "FREE_TRIAL_PERIOD_DAYS";
        }
    }
}

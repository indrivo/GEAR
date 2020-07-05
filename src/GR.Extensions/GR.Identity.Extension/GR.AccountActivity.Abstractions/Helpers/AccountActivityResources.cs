using System;

namespace GR.AccountActivity.Abstractions.Helpers
{
    public static class AccountActivityResources
    {
        public const string TrackActivityTokenProvider = "TrackActivityProvider";
        public const string ConfirmDevicePurpose = "confirm-device";
        public static TimeSpan TimeToDisableTrackingDevice = TimeSpan.FromDays(1);

        public static class ActivityTypes
        {
            public const string SIGNIN = "signin";
            public const string SIGNOUT = "signout";
            public const string SECOND_FACTOR_VERIFIED = "verified second factor";
            public const string DEVICE_CONFIRMED = "device confirmation completed";
            public const string PHONE_VERIFIED = "phone verification completed";
        }

        /// <summary>
        /// Get device cache key
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ip"></param>
        /// <param name="platform"></param>
        /// <param name="location"></param>
        /// <param name="browser"></param>
        /// <returns></returns>
        public static string GetDeviceCacheKey(Guid userId, string ip, string platform, string location, string browser)
        {
            return "device_" + userId + ip + platform + location + browser;
        }
    }
}
using System;
using GR.Core.Events;
using GR.TwoFactorAuthentication.Abstractions.Events.EventArgs;

namespace GR.TwoFactorAuthentication.Abstractions.Events
{
    public static class TwoFactorAuthEvents
    {
        public struct Events
        {
            #region Events

            /// <summary>
            /// On 2 factor verified
            /// </summary>
            public static event EventHandler<SecondFactorVerifiedEventArgs> On2FactorVerified;

            #endregion

            #region Triggers

            /// <summary>
            /// 2 factor verified trigger 
            /// </summary>
            /// <param name="e"></param>
            public static void TwoFactorVerified(SecondFactorVerifiedEventArgs e)
                => SystemEvents.InvokeEvent(null, On2FactorVerified, e, nameof(On2FactorVerified));
            #endregion
        }

        /// <summary>
        /// Register events
        /// </summary>
        public static void RegisterEvents()
        {
            SystemEvents.Common.RegisterEventGroup(nameof(TwoFactorAuthEvents), SystemEvents.GetEvents(typeof(Events)));
        }
    }
}

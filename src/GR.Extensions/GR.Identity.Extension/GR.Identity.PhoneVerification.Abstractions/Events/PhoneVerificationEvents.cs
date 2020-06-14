using System;
using GR.Core.Events;
using GR.Identity.PhoneVerification.Abstractions.Events.EventArgs;

namespace GR.Identity.PhoneVerification.Abstractions.Events
{
    public static class PhoneVerificationEvents
    {
        public struct Events
        {
            #region Events

            /// <summary>
            /// On device confirmed
            /// </summary>
            public static event EventHandler<PhoneConfirmedEventArgs> OnPhoneVerified;

            #endregion

            #region Triggers

            /// <summary>
            /// Device confirmed
            /// </summary>
            /// <param name="e"></param>
            public static void PhoneVerified(PhoneConfirmedEventArgs e)
                => SystemEvents.InvokeEvent(null, OnPhoneVerified, e, nameof(OnPhoneVerified));
            #endregion
        }

        /// <summary>
        /// Register events
        /// </summary>
        public static void RegisterEvents()
        {
            SystemEvents.Common.RegisterEventGroup(nameof(PhoneVerificationEvents), SystemEvents.GetEvents(typeof(Events)));
        }
    }
}
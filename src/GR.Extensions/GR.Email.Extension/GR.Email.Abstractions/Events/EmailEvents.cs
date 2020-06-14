using System;
using GR.Core.Events;
using GR.Email.Abstractions.Events.EventArgs;

namespace GR.Email.Abstractions.Events
{
    public static class EmailEvents
    {
        public struct Events
        {
            #region Events

            /// <summary>
            /// On email confirm
            /// </summary>
            public static event EventHandler<EmailConfirmEventArgs> OnEmailConfirmed;

            /// <summary>
            /// On confirm email sent
            /// </summary>
            public static event EventHandler<SendConfirmEmailEventArgs> OnConfirmEmailSend;

            #endregion

            #region Triggers

            public static void TriggerEmailConfirmed(EmailConfirmEventArgs e)
                => SystemEvents.InvokeEvent(null, OnEmailConfirmed, e, nameof(OnEmailConfirmed));

            public static void TriggerSendConfirmEmail(SendConfirmEmailEventArgs e)
                => SystemEvents.InvokeEvent(null, OnConfirmEmailSend, e, nameof(OnConfirmEmailSend));

            #endregion
        }

        /// <summary>
        /// Register events
        /// </summary>
        public static void RegisterEvents()
        {
            SystemEvents.Common.RegisterEventGroup(nameof(EmailEvents), SystemEvents.GetEvents(typeof(Events)));
        }
    }
}
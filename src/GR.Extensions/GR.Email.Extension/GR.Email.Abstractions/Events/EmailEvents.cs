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
            public static event EventHandler<EmailConfirmEventArgs> OnEmailConfirm;

            /// <summary>
            /// On confirm email sent
            /// </summary>
            public static event EventHandler<SendConfirmEmailEventArgs> OnConfirmEmailSend;

            #endregion

            #region Triggers

            public static void TriggerEmailConfirm(EmailConfirmEventArgs e)
                => SystemEvents.InvokeEvent(null, OnEmailConfirm, e, nameof(OnEmailConfirm));

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
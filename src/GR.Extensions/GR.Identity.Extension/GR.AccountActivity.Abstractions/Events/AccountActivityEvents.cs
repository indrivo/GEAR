using System;
using GR.AccountActivity.Abstractions.Events.EventArgs;
using GR.Core.Events;

namespace GR.AccountActivity.Abstractions.Events
{
    public static class AccountActivityEvents
    {
        public struct Events
        {
            #region Events

            /// <summary>
            /// On device confirmed
            /// </summary>
            public static event EventHandler<DeviceConfirmedEventArgs> OnDeviceConfirmed;

            #endregion

            #region Triggers

            /// <summary>
            /// Trigger device confirmed
            /// </summary>
            /// <param name="e"></param>
            public static void DeviceConfirmed(DeviceConfirmedEventArgs e)
                => SystemEvents.InvokeEvent(null, OnDeviceConfirmed, e, nameof(OnDeviceConfirmed));

            #endregion
        }

        public static void RegisterEvents()
        {
            SystemEvents.Common.RegisterEventGroup(nameof(AccountActivityEvents), SystemEvents.GetEvents(typeof(Events)));
        }
    }
}

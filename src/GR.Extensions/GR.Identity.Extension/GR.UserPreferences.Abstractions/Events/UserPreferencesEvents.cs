using System;
using GR.Core.Events;
using GR.UserPreferences.Abstractions.Events.EventArgs;

namespace GR.UserPreferences.Abstractions.Events
{
    public static class UserPreferencesEvents
    {
        public struct Preferences
        {
            #region Events

            /// <summary>
            /// On key updated
            /// </summary>
            public static event EventHandler<KeyUpdateEventArgs> OnKeyUpdated;

            #endregion Events

            #region Triggers

            /// <summary>
            /// On key updated
            /// </summary>
            /// <param name="e"></param>
            public static void KeyUpdated(KeyUpdateEventArgs e) => SystemEvents.InvokeEvent(null, OnKeyUpdated, e, nameof(OnKeyUpdated));

            #endregion Triggers
        }

        public static void RegisterEvents()
        {
            //register events on global storage
            SystemEvents.Common.RegisterEventGroup(nameof(UserPreferencesEvents), SystemEvents.GetEvents(typeof(Preferences)));
        }
    }
}
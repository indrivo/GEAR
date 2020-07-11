using System;
using GR.Core.Events;
using GR.Localization.Abstractions.Events.EventArgs;

namespace GR.Localization.Abstractions.Events
{
    public static class LocalizationEvents
    {
        public struct Languages
        {
            #region Events

            /// <summary>
            /// On language changed
            /// </summary>
            public static event EventHandler<ChangeLanguageEventArgs> OnLanguageChanged;

            #endregion

            #region Triggers

            /// <summary>
            /// Trigger language change
            /// </summary>
            /// <param name="e"></param>
            public static void ChangeLanguage(ChangeLanguageEventArgs e)
                => SystemEvents.InvokeEvent(null, OnLanguageChanged, e, nameof(OnLanguageChanged));

            #endregion
        }

        public static void RegisterEvents()
        {
            SystemEvents.Common.RegisterEventGroup(nameof(Languages), SystemEvents.GetEvents(typeof(Languages)));
        }
    }
}

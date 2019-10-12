using System;
using ST.Calendar.Abstractions.Events.EventArgs;
using ST.Core.Events;

namespace ST.Calendar.Abstractions.Events
{
    public static class CalendarEvents
    {
        public struct SystemCalendarEvents
        {
            #region Event Created
            /// <summary>
            /// On event created
            /// </summary>
            public static event EventHandler<CalendarEventCreatedEventArgs> OnEventCreated;

            /// <summary>
            /// Event trigger
            /// </summary>
            /// <param name="e"></param>
            public static void EventCreated(CalendarEventCreatedEventArgs e) => SystemEvents.InvokeEvent(null, OnEventCreated, e, nameof(OnEventCreated));
            #endregion

            #region Event updated

            /// <summary>
            /// On event updated
            /// </summary>
            public static event EventHandler<EventUpdatedEventArgs> OnEventUpdated;

            /// <summary>
            /// Event updated trigger
            /// </summary>
            /// <param name="e"></param>
            public static void EventUpdated(EventUpdatedEventArgs e) => SystemEvents.InvokeEvent(null, OnEventUpdated, e, nameof(OnEventUpdated));

            #endregion

            #region Event deleted

            /// <summary>
            /// On event deleted
            /// </summary>
            public static event EventHandler<EventDeleteOrRestoredEventArgs> OnEventDeleted;

            /// <summary>
            /// Event delete trigger
            /// </summary>
            /// <param name="e"></param>
            public static void EventDeleted(EventDeleteOrRestoredEventArgs e) => SystemEvents.InvokeEvent(null, OnEventDeleted, e, nameof(OnEventDeleted));

            #endregion

            #region Event restored

            /// <summary>
            /// On event restored
            /// </summary>
            public static event EventHandler<EventDeleteOrRestoredEventArgs> OnEventRestored;

            /// <summary>
            /// Event restore trigger
            /// </summary>
            /// <param name="e"></param>
            public static void EventRestored(EventDeleteOrRestoredEventArgs e) => SystemEvents.InvokeEvent(null, OnEventRestored, e, nameof(OnEventRestored));

            #endregion
        }


        /// <summary>
        /// Register events
        /// </summary>
        public static void RegisterEvents()
        {
            SystemEvents.Common.RegisterEventGroup(nameof(SystemCalendarEvents), SystemEvents.GetEvents(typeof(SystemCalendarEvents)));
        }
    }
}

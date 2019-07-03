using System;
using ST.PageRender.Abstractions.Events.EventArgs;

namespace ST.PageRender.Abstractions.Events
{
    public static class SystemEvents
    {
        public struct Pages
        {
            /// <summary>
            /// On page created
            /// </summary>
            public static event EventHandler<PageCreatedEventArgs> OnPageCreated;
            /// <summary>
            /// On page deleted
            /// </summary>
            public static event EventHandler<PageCreatedEventArgs> OnPageDeleted;
            /// <summary>
            /// On page updated
            /// </summary>
            public static event EventHandler<PageCreatedEventArgs> OnPageUpdated;

            /// <summary>
            /// Page created
            /// </summary>
            /// <param name="e"></param>
            public static void PageCreated(PageCreatedEventArgs e) => Core.Events.SystemEvents.InvokeEvent(null, OnPageCreated, e, nameof(OnPageCreated));
            /// <summary>
            /// On page deleted
            /// </summary>
            /// <param name="e"></param>
            public static void PageDeleted(PageCreatedEventArgs e) => Core.Events.SystemEvents.InvokeEvent(null, OnPageDeleted, e, nameof(OnPageDeleted));
            /// <summary>
            /// On page updated
            /// </summary>
            /// <param name="e"></param>
            public static void PageUpdated(PageCreatedEventArgs e) => Core.Events.SystemEvents.InvokeEvent(null, OnPageUpdated, e, nameof(OnPageUpdated));
        }
        /// <summary>
        /// Register app events
        /// </summary>
        public static void RegisterEvents()
        {
            //Pages
            Pages.OnPageCreated += EventHandlers.OnPageCreatedHandler;
            Pages.OnPageDeleted += EventHandlers.OnPageDeletedHandler;
            Pages.OnPageUpdated += EventHandlers.OnPageUpdatedHandler;
        }
    }
}

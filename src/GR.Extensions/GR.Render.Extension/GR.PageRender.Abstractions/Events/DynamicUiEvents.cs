using System;
using GR.Core.Events;
using GR.PageRender.Abstractions.Events.EventArgs;

namespace GR.PageRender.Abstractions.Events
{
    public static class DynamicUiEvents
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
            public static void PageCreated(PageCreatedEventArgs e) => SystemEvents.InvokeEvent(null, OnPageCreated, e, nameof(OnPageCreated));
            /// <summary>
            /// On page deleted
            /// </summary>
            /// <param name="e"></param>
            public static void PageDeleted(PageCreatedEventArgs e) => SystemEvents.InvokeEvent(null, OnPageDeleted, e, nameof(OnPageDeleted));
            /// <summary>
            /// On page updated
            /// </summary>
            /// <param name="e"></param>
            public static void PageUpdated(PageCreatedEventArgs e) => SystemEvents.InvokeEvent(null, OnPageUpdated, e, nameof(OnPageUpdated));
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

            //register events on global storage
            SystemEvents.Common.RegisterEventGroup(nameof(Pages), SystemEvents.GetEvents(typeof(Pages)));
        }
    }
}

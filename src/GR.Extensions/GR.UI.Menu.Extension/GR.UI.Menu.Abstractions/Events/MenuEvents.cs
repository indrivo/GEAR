using System;
using GR.Core.Events;
using GR.UI.Menu.Abstractions.Events.EventArgs;

namespace GR.UI.Menu.Abstractions.Events
{
    public static class MenuEvents
    {
        public struct Menu
        {
            /// <summary>
            /// On menu seed
            /// </summary>
            public static event EventHandler<MenuSeedEventArgs> OnMenuSeed;

            /// <summary>
            /// Menu seed
            /// </summary>
            /// <param name="e"></param>
            public static void MenuSeed(MenuSeedEventArgs e) => SystemEvents.InvokeEvent(null, OnMenuSeed, e, nameof(OnMenuSeed));
        }

        public static void RegisterEvents()
        {
            //register events on global storage
            SystemEvents.Common.RegisterEventGroup(nameof(Menu), SystemEvents.GetEvents(typeof(Menu)));
        }
    }
}

using System;
using ST.Core.Events.EventArgs;
using ST.Core.Extensions;

namespace ST.Core.Events
{
    public static class SystemEvents
    {
        public struct Application
        {
            /// <summary>
            /// On application start event
            /// </summary>
            public static event EventHandler<ApplicationStartedEventArgs> OnApplicationStarted;

            /// <summary>
            /// Application started handler
            /// </summary>
            /// <param name="e"></param>
            public static void ApplicationStarted(ApplicationStartedEventArgs e)
                => InvokeEvent(null, OnApplicationStarted, e, nameof(OnApplicationStarted));

            /// <summary>
            /// On application stop event
            /// </summary>
            public static event EventHandler<ApplicationStopEventArgs> OnApplicationStopped;

            /// <summary>
            /// Application stopped handler
            /// </summary>
            /// <param name="e"></param>
            public static void ApplicationStopped(ApplicationStopEventArgs e) => InvokeEvent(null, OnApplicationStopped, e, nameof(OnApplicationStopped));

            /// <summary>
            /// On application event
            /// </summary>
            public static event EventHandler<ApplicationEventEventArgs> OnEvent;
            /// <summary>
            /// Application event
            /// </summary>
            /// <param name="e"></param>
            public static void Event(ApplicationEventEventArgs e) => InvokeEvent(null, OnEvent, e, nameof(OnEvent));
        }

        /// <summary>
        /// Register app events
        /// </summary>
        public static void RegisterEvents()
        {
            Application.OnApplicationStarted += EventHandlers.OnApplicationStartedHandler;
            Application.OnApplicationStopped += EventHandlers.OnApplicationStoppedHandler;
            Application.OnEvent += EventHandlers.OnEventHandler;
        }

        /// <summary>
        /// Invoke events
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="sender"></param>
        /// <param name="evt"></param>
        /// <param name="e"></param>
        /// <param name="eventName"></param>
        public static void InvokeEvent<TEventArgs>(object sender, EventHandler<TEventArgs> evt, TEventArgs e, string eventName = nameof(EventHandler))
            where TEventArgs : System.EventArgs
        {
            if (evt == null) return;
            if (eventName != nameof(Application.OnEvent))
            {
                Console.Write("Event ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(eventName);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" was invoked");
                Application.Event(new ApplicationEventEventArgs
                {
                    EventArgs = e.ToDictionary(),
                    EventDate = DateTime.Now,
                    EventName = eventName
                });
            }
            evt.Invoke(sender, e);
        }
    }
}

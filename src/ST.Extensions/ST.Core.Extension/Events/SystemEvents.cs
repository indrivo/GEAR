using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ST.Core.Events.EventArgs;
using ST.Core.Extensions;
using ST.Core.Helpers;

namespace ST.Core.Events
{
    public static class SystemEvents
    {
        public static class Common
        {
            /// <summary>
            /// Store all system events
            /// </summary>
            private static readonly ConcurrentDictionary<string, IEnumerable<string>> Events = new ConcurrentDictionary<string, IEnumerable<string>>();

            /// <summary>
            /// Get registered events
            /// </summary>
            public static Dictionary<string, IEnumerable<string>> RegisteredEvents => Events.ToDictionary(k => k.Key, v => v.Value);

            /// <summary>
            /// Register event group
            /// </summary>
            /// <param name="groupName"></param>
            /// <param name="events"></param>
            /// <returns></returns>
            public static bool RegisterEventGroup(string groupName, IEnumerable<string> events)
            {
                return Events.TryAdd(groupName, events);
            }

            /// <summary>
            /// Get events from group
            /// </summary>
            /// <param name="groupName"></param>
            /// <returns></returns>
            public static IEnumerable<string> GetGroupEvents(string groupName)
            {
                Events.TryGetValue(groupName, out var events);
                return events;
            }

            /// <summary>
            /// Check if system has some event
            /// </summary>
            /// <param name="eventName"></param>
            /// <returns></returns>
            public static bool HasEvent(string eventName)
            {
                return Events.Any(x => x.Value.Contains(eventName));
            }
        }

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

            //register event group
            Common.RegisterEventGroup(nameof(Application), GetEvents(typeof(Application)));
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

        /// <summary>
        /// Get events
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetEvents(Type type)
        {
            Arg.NotNull(type, nameof(GetEvents));
            var props = type.GetEvents()
                .Select(x => x.Name).ToList();

            return props;
        }
    }
}

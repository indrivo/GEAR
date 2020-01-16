using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GR.Core.Events.EventArgs;
using GR.Core.Events.EventArgs.Database;
using GR.Core.Extensions;
using GR.Core.Helpers;

namespace GR.Core.Events
{
    public static class SystemEvents
    {
        public static class Common
        {
            /// <summary>
            /// Store all system events
            /// </summary>
            private static readonly ConcurrentDictionary<string, Dictionary<string, IEnumerable<string>>> Events =
                new ConcurrentDictionary<string, Dictionary<string, IEnumerable<string>>>();

            /// <summary>
            /// Get registered events
            /// </summary>
            public static Dictionary<string, IEnumerable<string>> RegisteredEvents
                => Events.ToDictionary(k => k.Key, v => v.Value.Keys as IEnumerable<string>);

            /// <summary>
            /// Get props of event
            /// </summary>
            /// <param name="eventName"></param>
            /// <returns></returns>
            public static IEnumerable<string> GetEventPropNames(string eventName)
            {
                foreach (var group in Events)
                {
                    foreach (var ev in group.Value)
                    {
                        if (ev.Key.Equals(eventName))
                        {
                            return ev.Value;
                        }
                    }
                }
                return new List<string>();
            }

            /// <summary>
            /// Register event group
            /// </summary>
            /// <param name="groupName"></param>
            /// <param name="events"></param>
            /// <returns></returns>
            public static bool RegisterEventGroup(string groupName, Dictionary<string, IEnumerable<string>> events)
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
                return events?.Keys;
            }

            /// <summary>
            /// Check if system has some event
            /// </summary>
            /// <param name="eventName"></param>
            /// <returns></returns>
            public static bool HasEvent(string eventName)
            {
                return Events.Any(x => x.Value.Keys.Contains(eventName));
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

        public struct Database
        {
            /// <summary>
            /// On seed event
            /// </summary>
            public static event EventHandler<DatabaseSeedEventArgs> OnSeed;

            /// <summary>
            /// Invoke seed event
            /// </summary>
            /// <param name="e"></param>
            public static void Seed(DatabaseSeedEventArgs e) => InvokeEvent(null, OnSeed, e, nameof(OnSeed));


            /// <summary>
            /// On seed event
            /// </summary>
            public static event EventHandler<DatabaseMigrateEventArgs> OnMigrate;

            /// <summary>
            /// Invoke seed event
            /// </summary>
            /// <param name="e"></param>
            public static void Migrate(DatabaseMigrateEventArgs e) => InvokeEvent(null, OnMigrate, e, nameof(OnMigrate));

            /// <summary>
            /// On seed event
            /// </summary>
            public static event EventHandler<DatabaseMigrateEventArgs> OnMigrateComplete;

            /// <summary>
            /// Invoke seed event
            /// </summary>
            /// <param name="e"></param>
            public static void MigrateComplete(DatabaseMigrateEventArgs e) => InvokeEvent(null, OnMigrateComplete, e, nameof(OnMigrateComplete));
        }

        /// <summary>
        /// Register app events
        /// </summary>
        public static void RegisterEvents()
        {
            Application.OnApplicationStarted += EventHandlers.OnApplicationStartedHandler;
            Application.OnApplicationStopped += EventHandlers.OnApplicationStoppedHandler;
            Database.OnMigrateComplete += EventHandlers.OnMigrationCompleteHandler;

            //register event group
            Common.RegisterEventGroup(nameof(Application), GetEvents(typeof(Application)));
            Common.RegisterEventGroup(nameof(Database), GetEvents(typeof(Database)));
        }

        #region Helpers

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
                    EventArgs = e.ToDictionary()
                        .ToDictionary(x => x.Key, c => c.Value),
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
        public static Dictionary<string, IEnumerable<string>> GetEvents(Type type)
        {
            Arg.NotNull(type, nameof(GetEvents));
            return type.GetEvents().ToDictionary(ev => ev.Name, GetEventProps);
        }

        /// <summary>
        /// Get event handler type props
        /// </summary>
        /// <param name="eventInfo"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetEventProps(EventInfo eventInfo)
        {
            var type = eventInfo.EventHandlerType.GetMethod("Invoke")
                ?.GetParameters()[1].
                ParameterType;

            return type?.GetProperties().Select(x => x.Name);
        }

        #endregion
    }
}
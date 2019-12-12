using System;
using GR.Core.Events;
using GR.Entities.Abstractions.Events.EventArgs;

namespace GR.Entities.Abstractions.Events
{
    public static class EntityEvents
    {
        public struct SqlQuery
        {
            /// <summary>
            /// On query executed
            /// </summary>
            public static event EventHandler<ExecutedQueryEventArgs> OnQueryExecuted;
            /// <summary>
            /// Rise new query executed
            /// </summary>
            /// <param name="e"></param>
            public static void QueryExecuted(ExecutedQueryEventArgs e) => SystemEvents.InvokeEvent(null, OnQueryExecuted, e, nameof(OnQueryExecuted));
        }

        public struct Entities
        {
            public static event EventHandler<EntityCreatedEventArgs> OnEntityCreated;
            public static event EventHandler<EntityDeleteEventArgs> OnEntityDeleted;
            public static event EventHandler<EntityUpdateEventArgs> OnEntityUpdated;

            public static event EventHandler<EntityAddNewFieldEventArgs> OnEntityAddNewField;
            public static event EventHandler<EntityUpdateFieldEventArgs> OnEntityUpdateField;
            public static event EventHandler<EntityDeleteEventArgs> OnEntityDeleteField;

            //entity
            public static void EntityCreated(EntityCreatedEventArgs e) => SystemEvents.InvokeEvent(null, OnEntityCreated, e, nameof(OnEntityCreated));
            public static void EntityDeleted(EntityDeleteEventArgs e) => SystemEvents.InvokeEvent(null, OnEntityDeleted, e, nameof(OnEntityDeleted));
            public static void EntityUpdated(EntityUpdateEventArgs e) => SystemEvents.InvokeEvent(null, OnEntityUpdated, e, nameof(OnEntityUpdated));

            //entity fields
            public static void EntityAddNewField(EntityAddNewFieldEventArgs e) => SystemEvents.InvokeEvent(null, OnEntityAddNewField, e, nameof(OnEntityAddNewField));
            public static void EntityUpdateField(EntityUpdateFieldEventArgs e) => SystemEvents.InvokeEvent(null, OnEntityUpdateField, e, nameof(OnEntityUpdateField));
            public static void EntityDeleteField(EntityDeleteEventArgs e) => SystemEvents.InvokeEvent(null, OnEntityDeleteField, e, nameof(OnEntityDeleteField));
        }

        /// <summary>
        /// Register events
        /// </summary>
        public static void RegisterEvents()
        {
            SqlQuery.OnQueryExecuted += EventHandlers.OnQueryExecutedHandler;

            //register events on global storage
            SystemEvents.Common.RegisterEventGroup(nameof(SqlQuery), SystemEvents.GetEvents(typeof(SqlQuery)));
            SystemEvents.Common.RegisterEventGroup(nameof(Entities), SystemEvents.GetEvents(typeof(Entities)));
        }
    }
}

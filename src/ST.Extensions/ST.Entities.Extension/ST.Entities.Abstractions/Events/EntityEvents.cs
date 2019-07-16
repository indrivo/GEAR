using System;
using ST.Entities.Abstractions.Events.EventArgs;

namespace ST.Entities.Abstractions.Events
{
    public static class EntityEvents
    {
        /// <summary>
        /// On query executed
        /// </summary>
        public static event EventHandler<ExecutedQueryEventArgs> OnQueryExecuted;
        /// <summary>
        /// Rise new query executed
        /// </summary>
        /// <param name="e"></param>
        public static void QueryExecuted(ExecutedQueryEventArgs e) => Core.Events.SystemEvents.InvokeEvent(null, OnQueryExecuted, e, nameof(OnQueryExecuted));

        /// <summary>
        /// Register events
        /// </summary>
        public static void RegisterEvents()
        {
            OnQueryExecuted += EventHandlers.OnQueryExecutedHandler;
        }
    }
}

using System;
using ST.ECommerce.Abstractions.Events.EventArgs;

namespace ST.ECommerce.Abstractions.Events
{
    public static class CommerceEvents
    {
        /// <summary>
        /// On new product added
        /// </summary>
        public static event EventHandler<ProductAddedEventArgs> OnProductAdded;
        /// <summary>
        /// Rise new product added
        /// </summary>
        /// <param name="e"></param>
        public static void QueryExecuted(ProductAddedEventArgs e) => Core.Events.SystemEvents.InvokeEvent(null, OnProductAdded, e, nameof(OnProductAdded));

        /// <summary>
        /// Register events
        /// </summary>
        public static void RegisterEvents()
        {
            OnProductAdded += CommerceEventHandlers.OnProductAddedHandler;
        }
    }
}

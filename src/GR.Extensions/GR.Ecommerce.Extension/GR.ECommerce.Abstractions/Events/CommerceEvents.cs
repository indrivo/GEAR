﻿using System;
using GR.Core.Events;
using GR.ECommerce.Abstractions.Events.EventArgs.ProductsEventArgs;

namespace GR.ECommerce.Abstractions.Events
{
    public static class CommerceEvents
    {
        public struct Products
        {
            /// <summary>
            /// On new product added
            /// </summary>
            public static event EventHandler<ProductAddedEventArgs> OnProductAdded;
            /// <summary>
            /// Rise new product added
            /// </summary>
            /// <param name="e"></param>
            public static void ProductAdded(ProductAddedEventArgs e) => SystemEvents.InvokeEvent(null, OnProductAdded, e, nameof(OnProductAdded));
        }

        /// <summary>
        /// Register events
        /// </summary>
        public static void RegisterEvents()
        {
            Products.OnProductAdded += CommerceEventHandlers.OnProductAddedHandler;

            SystemEvents.Common.RegisterEventGroup(nameof(Products), SystemEvents.GetEvents(typeof(Products)));
        }
    }
}

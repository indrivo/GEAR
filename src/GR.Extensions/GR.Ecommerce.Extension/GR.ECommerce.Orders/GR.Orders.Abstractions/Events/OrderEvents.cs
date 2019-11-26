using System;
using GR.Core.Events;
using GR.Orders.Abstractions.Events.EventArgs.OrderEventArgs;

namespace GR.Orders.Abstractions.Events
{
    public class OrderEvents
    {
        public struct Orders
        {
            /// <summary>
            /// On new product added
            /// </summary>
            public static event EventHandler<AddOrderEventArgs> OnOrderCreated;

            /// <summary>
            /// Rise new product added
            /// </summary>
            /// <param name="e"></param>
            public static void OrderCreated(AddOrderEventArgs e) => SystemEvents.InvokeEvent(null, OnOrderCreated, e, nameof(OnOrderCreated));

            /// <summary>
            /// On payment received
            /// </summary>
            public static event EventHandler<PaymentReceivedEventArgs> OnPaymentReceived;

            /// <summary>
            /// Trigger event
            /// </summary>
            /// <param name="e"></param>
            public static void PaymentReceived(PaymentReceivedEventArgs e) => SystemEvents.InvokeEvent(null, OnPaymentReceived, e, nameof(OnPaymentReceived));
        }

        /// <summary>
        /// Register events
        /// </summary>
        public static void RegisterEvents()
        {
            SystemEvents.Common.RegisterEventGroup(nameof(Orders), SystemEvents.GetEvents(typeof(Orders)));
        }
    }
}
using System;

namespace GR.Orders.Abstractions.Events.EventArgs.OrderEventArgs
{
    public class PaymentReceivedEventArgs : System.EventArgs
    {
        public Guid OrderId { get; set; }
    }
}

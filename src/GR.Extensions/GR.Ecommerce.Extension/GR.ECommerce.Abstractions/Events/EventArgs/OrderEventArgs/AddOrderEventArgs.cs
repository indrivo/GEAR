using System;

namespace GR.ECommerce.Abstractions.Events.EventArgs.OrderEventArgs
{
    public class AddOrderEventArgs : System.EventArgs
    {
        public Guid Id { get; set; }
        public string OrderStatus { get; set; }
    }
}

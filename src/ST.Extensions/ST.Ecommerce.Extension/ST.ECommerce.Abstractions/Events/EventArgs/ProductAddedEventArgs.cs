using System;

namespace ST.ECommerce.Abstractions.Events.EventArgs
{
    public class ProductAddedEventArgs : System.EventArgs
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}

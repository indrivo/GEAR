using System;

namespace GR.ECommerce.Abstractions.Events.EventArgs.ProductsEventArgs
{
    public class ProductAddedEventArgs : System.EventArgs
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}

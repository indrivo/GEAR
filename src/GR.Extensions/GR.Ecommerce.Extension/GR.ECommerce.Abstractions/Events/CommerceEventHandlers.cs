using GR.ECommerce.Abstractions.Events.EventArgs;
using GR.ECommerce.Abstractions.Events.EventArgs.ProductsEventArgs;

namespace GR.ECommerce.Abstractions.Events
{
    public static class CommerceEventHandlers
    {
        public static void OnProductAddedHandler(object sender, ProductAddedEventArgs args)
        {
            //Do something
        }
    }
}

using GR.ECommerce.Abstractions.Events.EventArgs;

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

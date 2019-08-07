using ST.ECommerce.Abstractions.Events.EventArgs;

namespace ST.ECommerce.Abstractions.Events
{
    public static class CommerceEventHandlers
    {
        public static void OnProductAddedHandler(object sender, ProductAddedEventArgs args)
        {
            //Do something
        }
    }
}

using GR.PageRender.Abstractions.Events.EventArgs;

namespace GR.PageRender.Abstractions.Events
{
    public static class EventHandlers
    {
        /// <summary>
        /// On application start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnPageCreatedHandler(object sender, PageCreatedEventArgs args)
        {
            //Do something
        }

        /// <summary>
        /// On page deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnPageDeletedHandler(object sender, PageCreatedEventArgs args)
        {
            //Do something
        }

        /// <summary>
        /// On page updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnPageUpdatedHandler(object sender, PageCreatedEventArgs args)
        {
            //Do something
        }
    }
}

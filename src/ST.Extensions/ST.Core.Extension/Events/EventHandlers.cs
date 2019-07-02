using System;
using ST.Core.Events.EventArgs;

namespace ST.Core.Events
{
    public static class EventHandlers
    {
        /// <summary>
        /// On application start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnApplicationStartedHandler(object sender, ApplicationStartedEventArgs args)
        {
            Console.WriteLine(args.AppIdentifier);
        }

        /// <summary>
        /// On application stop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnApplicationStoppedHandler(object sender, ApplicationStopEventArgs args)
        {
            Console.WriteLine(args.AppIdentifier);
        }

        /// <summary>
        /// On event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnEventHandler(object sender, ApplicationEventEventArgs args)
        {
            //Do something
        }
    }
}

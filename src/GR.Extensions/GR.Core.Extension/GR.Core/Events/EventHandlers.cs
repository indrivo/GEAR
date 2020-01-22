using System;
using GR.Core.Events.EventArgs;
using GR.Core.Events.EventArgs.Database;

namespace GR.Core.Events
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
        /// On migration complete event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnMigrationCompleteHandler(object sender, DatabaseMigrateEventArgs args)
        {
            //Trigger seed data
            Console.WriteLine($"Context {args.ContextName}");
            SystemEvents.Database.Seed(new DatabaseSeedEventArgs
            {
                DbContext = args.DbContext,
                ContextName = args.ContextName
            });
        }
    }
}

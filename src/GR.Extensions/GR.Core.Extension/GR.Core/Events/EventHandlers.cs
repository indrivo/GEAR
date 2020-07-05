using System;
using GR.Core.Events.EventArgs;
using GR.Core.Events.EventArgs.Database;
using GR.Core.Helpers;

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
            ConsoleWriter.WriteTextAsTitle($"App {args.AppIdentifier} started", ConsoleColor.DarkMagenta);
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
            Console.WriteLine($"Context {args.ContextName}");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Context {args.ContextName} migrated in {TimeSpan.FromMilliseconds(args.ElapsedMilliseconds).TotalMinutes} minutes");
            Console.ForegroundColor = ConsoleColor.White;

            //Trigger seed data
            SystemEvents.Database.Seed(new DatabaseSeedEventArgs
            {
                DbContext = args.DbContext,
                ContextName = args.ContextName
            });
        }
    }
}

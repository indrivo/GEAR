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
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            var topAndBottom = new string('-', Console.WindowWidth - 1);
            Console.Write(topAndBottom + "\n");
            Console.Write(topAndBottom + "\n");
            var padding = new string('-', (Console.WindowWidth - args.AppIdentifier.Length) / 2);
            Console.Write(padding);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(args.AppIdentifier);
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(padding + "\n");
            Console.Write(topAndBottom + "\n");
            Console.Write(topAndBottom + "\n");
            Console.ForegroundColor = ConsoleColor.White;
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

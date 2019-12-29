using System;
using GR.Entities.Abstractions.Events.EventArgs;

namespace GR.Entities.Abstractions.Events
{
    public static class EventHandlers
    {
        /// <summary>
        /// On application start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnQueryExecutedHandler(object sender, ExecutedQueryEventArgs args)
        {
            if (args == null) return;
            if (args.Completed)
            {
                Console.WriteLine($"Query {args.Query} executed in {args.Elapsed} ms");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Query {args.Query} throw an exception: {args.Exception}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}

using System;

namespace GR.Core.Helpers
{
    public static class ConsoleWriter
    {
        /// <summary>
        /// Write text as title
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public static void WriteTextAsTitle(string text, ConsoleColor color)
        {
            try
            {
                Console.ForegroundColor = color;
                var topAndBottom = new string('-', Console.WindowWidth - 1);
                Console.Write(topAndBottom + "\n");
                Console.Write(topAndBottom + "\n");
                var padding = new string('-', (Console.WindowWidth - text.Length) / 2);
                Console.Write(padding);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(text);
                Console.ForegroundColor = color;
                Console.Write(padding + "\n");
                Console.Write(topAndBottom + "\n");
                Console.Write(topAndBottom + "\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Colored write from new line
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public static void ColoredWriteLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        /// <summary>
        /// Colored write
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public static void ColoredWrite(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
    }
}
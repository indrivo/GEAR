using System;
using System.Linq;

namespace ST.Core.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Split string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string[] Split(this string str, string delimiter)
        {
            if (string.IsNullOrEmpty(str)) return new[] { str };
            return str.Split(new[] { delimiter }, StringSplitOptions.None);
        }

        /// <summary>
        /// First char to upper
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        /// <summary>
        /// Is Null or empty snippet
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
    }
}

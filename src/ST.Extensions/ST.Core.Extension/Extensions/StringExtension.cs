using System;
using System.Linq;
using System.Text.RegularExpressions;

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
        public static string[] Split(this string str, string delimiter) =>
            string.IsNullOrEmpty(str)
                ? new[] { str }
                : str.Split(new[] { delimiter }, StringSplitOptions.None);

        /// <summary>
        /// First char to upper
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length < 2) return input;
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        /// <summary>
        /// Is Null or empty snippet
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        /// <summary>
        /// Is valid email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(this string email) => !email.IsNullOrEmpty() && Regex.IsMatch(email, GlobalResources.RegularExpressions.EMAIL);
    }
}

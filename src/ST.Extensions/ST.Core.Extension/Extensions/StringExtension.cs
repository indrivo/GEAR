using System;

namespace ST.Core.Extensions
{
    public static class StringExtension
    {
        public static string[] Split(this string str, string delimiter)
        {
            return str.Split(new[] { delimiter }, StringSplitOptions.None);
        }
    }
}

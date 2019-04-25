using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ST.Shared.Extensions
{
    public static class GuidExtensions
    {
        /// <summary>
        /// Check if is string is value
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsGuid(this string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            var guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");
            return guidRegEx.IsMatch(str);
        }
    }
}

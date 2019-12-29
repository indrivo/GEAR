using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GR.Core.Extensions
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
            var guidRegEx = new Regex(GlobalResources.RegularExpressions.GUID);
            return guidRegEx.IsMatch(str);
        }
        /// <summary>
        /// Parse string to Guid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string id)
        {
            if (string.IsNullOrEmpty(id)) return Guid.Empty;
            try
            {
                return Guid.Parse(id);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Parse string to Guid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Guid? TryToGuid(this string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                return Guid.Parse(id);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }
    }
}

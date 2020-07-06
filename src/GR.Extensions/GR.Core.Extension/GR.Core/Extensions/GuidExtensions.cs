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

        /// <summary>
        /// Guid to int 32
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public static int ToInt32(Guid uuid)
        {
            var gb = uuid.ToByteArray();
            return BitConverter.ToInt32(gb, 0);
        }

        /// <summary>
        /// Guid to long
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public static long ToLong(Guid uuid)
        {
            var gb = uuid.ToByteArray();
            return BitConverter.ToInt64(gb, 0);
        }
    }
}

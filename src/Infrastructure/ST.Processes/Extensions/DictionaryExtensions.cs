using Newtonsoft.Json;
using System.Collections.Generic;

namespace ST.Procesess.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Get bpm element settings as string
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static string ToStringSettings(this Dictionary<string, string> dictionary)
        {
            if (dictionary == null) return string.Empty;
            try
            {
                return JsonConvert.SerializeObject(dictionary);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}

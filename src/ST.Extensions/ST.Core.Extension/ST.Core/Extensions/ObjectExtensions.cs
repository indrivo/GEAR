using System;
using Newtonsoft.Json;

namespace ST.Core.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Serialize object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TOutput Deserialize<TOutput>(this string source) where TOutput : class
        {
            if (source.IsNullOrEmpty()) return null;
            try
            {
                return JsonConvert.DeserializeObject<TOutput>(source);
            }
            catch
            {
                //Ignore
            }

            return null;
        }
    }
}

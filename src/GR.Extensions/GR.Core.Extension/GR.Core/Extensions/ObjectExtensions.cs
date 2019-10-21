using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace GR.Core.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Serialize settings
        /// </summary>
        private static readonly JsonSerializerSettings SerializeSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        /// <summary>
        /// Serialize object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(this object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, SerializeSettings);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return string.Empty;
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
                return JsonConvert.DeserializeObject<TOutput>(source, SerializeSettings);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }
    }
}

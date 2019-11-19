using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GR.Core.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Serialize settings
        /// </summary>
        private static readonly JsonSerializerSettings SerializeSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        /// <summary>
        /// Serialize object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        public static string SerializeAsJson(this object obj, JsonSerializerSettings serializerSettings = null)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, serializerSettings ?? SerializeSettings);
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
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        public static TOutput Deserialize<TOutput>(this string source, JsonSerializerSettings serializerSettings = null) where TOutput : class
        {
            if (source.IsNullOrEmpty()) return null;
            try
            {
                return JsonConvert.DeserializeObject<TOutput>(source, serializerSettings ?? SerializeSettings);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }
    }
}

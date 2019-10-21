using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace GR.DynamicEntityStorage.Extensions
{
    public  static class GenericConverterExtension
    {
        /// <summary>
        /// Type to another type
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TOutput To<TInput, TOutput>(this TInput obj)
        {
            try
            {
                var serialize = JsonConvert.SerializeObject(obj);
                return string.IsNullOrEmpty(serialize) ? default : JsonConvert.DeserializeObject<TOutput>(serialize);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                return default;
            }
        }
    }
}

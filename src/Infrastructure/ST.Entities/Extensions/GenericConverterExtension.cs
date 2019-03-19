using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ST.Entities.Extensions
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
                if (string.IsNullOrEmpty(serialize)) return default(TOutput);
                return JsonConvert.DeserializeObject<TOutput>(serialize);
            }
            catch(Exception ex)
            {
                return default(TOutput);
            }
        }
    }
}

using System.Collections.Generic;

namespace ST.Core.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Add range new props to dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="context"></param>
        /// <param name="newItems"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> AddRange<TKey, TValue>(this Dictionary<TKey, TValue> context, Dictionary<TKey, TValue> newItems)
        {
            if (context == null) context = new Dictionary<TKey, TValue>();
            foreach (var (key, value) in newItems)
            {
                context.Add(key, value);
            }

            return context;
        }
    }
}

using System.Collections.Generic;

namespace GR.Core.Extensions
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
            foreach (var item in newItems)
            {
                if (context.ContainsKey(item.Key))
                {
                    context[item.Key] = item.Value;
                }
                else
                {
                    context.Add(item.Key, item.Value);
                }
            }

            return context;
        }

        /// <summary>
        /// Remove keys
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dict"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> RemoveKeys<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                if (dict.ContainsKey(key))
                {
                    dict.Remove(key);
                }
            }
            return dict;
        }
    }
}

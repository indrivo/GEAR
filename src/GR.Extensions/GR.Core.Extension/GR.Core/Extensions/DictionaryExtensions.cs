using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Get dictionary value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static T GetValue<T>(this IDictionary<string, object> source, string property)
        {
            if (source == null) return default;
            var item = source.FirstOrDefault(x => x.Key.Equals(property));
            return item.IsNull() ? default : item.Is<T>();
        }

        /// <summary>
        /// Get index of item
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int IndexOf(this IDictionary dictionary, object value)
        {
            for (var i = 0; i < dictionary.Count; ++i)
            {
                if (dictionary[i] == value) return i;
            }
            return -1;
        }
    }
}

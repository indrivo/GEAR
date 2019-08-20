using System;
using System.Collections.Generic;
using System.Linq;

namespace ST.Core.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Add range for hash list
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static HashSet<TTarget> AddRange<TTarget>(this HashSet<TTarget> context, IEnumerable<TTarget> data)
        {
            if (context == null) context = new HashSet<TTarget>();
            foreach (var item in data)
            {
                context.Add(item);
            }
            return context;
        }

        /// <summary>
        /// Replace item in source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> enumerable, int index, T value)
        {
            var current = 0;
            foreach (var item in enumerable)
            {
                yield return current == index ? value : item;
                current++;
            }
        }

        /// <summary>
        /// Distinct by propriety
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}

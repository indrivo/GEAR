using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;

namespace GR.Core.Extensions
{
    public static class CollectionExtensions
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
        /// Get distinct items by list propriety
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
            foreach (var element in source.Where(element => seenKeys.Add(keySelector(element)))) yield return element;
        }

        /// <summary>
        /// Join as string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> source, string separator)
        {
            return source == null ? string.Empty : string.Join(separator, source);
        }

        /// <summary>
        /// Is last
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsLast<T>(this IEnumerable<T> items, T item)
        {
            var list = items?.ToList() ?? new List<T>();
            if (!list.Any())
                return false;
            var last = list.ElementAt(list.Count - 1);
            return item.Equals(last);
        }

        /// <summary>
        /// Is first
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsFirst<T>(this IEnumerable<T> items, T item)
        {
            var list = items?.ToList() ?? new List<T>();
            if (!list.Any())
                return false;
            var first = list.FirstOrDefault();
            return item.Equals(first);
        }

        /// <summary>
        /// Get differences from 2 list 
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static (IList<TItem>, IList<TItem>) GetDifferences<TItem>(this IEnumerable<TItem> source, IEnumerable<TItem> target)
        {
            var aData = source?.ToList() ?? new List<TItem>();
            var bData = target?.ToList() ?? new List<TItem>();
            var toExclude = aData.Where(left => !bData.Contains(left)).ToList();
            var toAdd = bData.Where(right => !aData.Contains(right)).ToList();
            return (toAdd, toExclude);
        }

        /// <summary>
        /// Contains any
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ContainsAny<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> target)
        {
            if (source == null || target == null) return false;
            var enumeratedSource = source.ToList();
            var enumeratedTarget = target.ToList();
            if (!enumeratedSource.Any() || !enumeratedTarget.Any()) return false;
            var common = enumeratedSource.Intersect(enumeratedTarget);
            return common.Any();
        }

        /// <summary>
        /// Any start with
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool AnyStartWith(this IEnumerable<string> source, IEnumerable<string> target)
            => source.Any(x => target.Any(y => x.StartsWith(y, StringComparison.InvariantCultureIgnoreCase)));

        /// <summary>
        /// Any start with
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool AnyStartWith(this IEnumerable<string> source, string target)
            => source.Any(x => x.StartsWith(target, StringComparison.InvariantCultureIgnoreCase));

        /// <summary>
        /// NameValueCollection to KeyValuePair
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePair(this NameValueCollection collection)
        {
            return collection.AllKeys.Select(x => new KeyValuePair<string, string>(x, collection[x]));
        }

        /// <summary>
        /// To observable collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> coll)
        {
            var c = new ObservableCollection<T>();
            foreach (var e in coll)
                c.Add(e);
            return c;
        }

        /// <summary>
        /// Randomize collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> target)
        {
            var r = new Random();
            return target.OrderBy(x => (r.Next()));
        }

        /// <summary>
        /// Transpose
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> collection)
        {
            var values = collection?.ToList() ?? new List<IEnumerable<T>>();
            if (!values.Any())
                return values;
            if (!values.First().Any())
                return Transpose(values.Skip(1));

            var x = values.First().First();
            var xs = values.First().Skip(1);
            var xss = values.Skip(1).ToList();
            return
                new[] {new[] {x}
                        .Concat(xss.Select(ht => ht.First()))}
                    .Concat(new[] { xs }
                        .Concat(xss.Select(ht => ht.Skip(1)))
                        .Transpose());
        }

        /// <summary>
        /// To collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static Collection<T> ToCollection<T>(this IEnumerable<T> enumerable)
        {
            var collection = new Collection<T>();
            foreach (var i in enumerable)
                collection.Add(i);
            return collection;
        }


        /// <summary>
        ///   Returns all combinations of a chosen amount of selected elements in the sequence.
        /// </summary>
        /// <typeparam name = "T">The type of the elements of the input sequence.</typeparam>
        /// <param name = "source">The source for this extension method.</param>
        /// <param name = "select">The amount of elements to select for every combination.</param>
        /// <param name = "repetition">True when repetition of elements is allowed.</param>
        /// <returns>All combinations of a chosen amount of selected elements in the sequence.</returns>
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> source, int select, bool repetition = false)
        {
            Contract.Requires(source != null);
            Contract.Requires(select >= 0);

            var enumerable = source.ToList();
            return select == 0
                ? new[] { new T[0] }
                : enumerable.SelectMany((element, index) =>
                    enumerable
                        .Skip(repetition ? index : index + 1)
                        .Combinations(select - 1, repetition)
                        .Select(c => new[] { element }.Concat(c)));
        }
    }
}

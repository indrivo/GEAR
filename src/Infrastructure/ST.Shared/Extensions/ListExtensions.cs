using System.Collections.Generic;

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
    }
}

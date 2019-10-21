using System;
using System.Collections.Generic;
using System.Text;

namespace GR.Core.Extensions
{
    public static class NullExtensions
    {
        /// <summary>
        /// Is null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        /// <summary>
        /// Check if KeyValue is null
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNull<TK, TV>(this KeyValuePair<TK, TV> source)
        {
            return source.Equals(default(KeyValuePair<TK, TV>));
        }
    }
}

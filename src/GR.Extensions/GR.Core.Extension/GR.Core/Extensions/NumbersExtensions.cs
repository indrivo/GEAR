using System;

namespace GR.Core.Extensions
{
    public static class NumbersExtensions
    {
        /// <summary>
        /// Are float numbers equal
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCompare"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool AreEqual(this float source, float toCompare, float epsilon = float.Epsilon)
        {
            return Math.Abs(source - toCompare) < epsilon;
        }

        /// <summary>
        /// Are double numbers equal
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCompare"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool AreEqual(this double source, double toCompare, double epsilon = double.Epsilon)
        {
            return Math.Abs(source - toCompare) < epsilon;
        }

    }
}

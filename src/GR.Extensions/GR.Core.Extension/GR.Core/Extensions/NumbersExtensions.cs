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

        /// <summary>
        /// Get a certain percentage of the specified number.
        /// </summary>
        /// <param name="value">The number to get the percentage of.</param>
        /// <param name="percentage">The percentage of the specified number to get.</param>
        /// <returns>The actual specified percentage of the specified number.</returns>
        public static double GetPercentage(this double value, int percentage)
        {
            var percentAsDouble = (double)percentage / 100;
            return value * percentAsDouble;
        }

        /// <summary>
        /// Get percent of
        /// </summary>
        /// <param name="value"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static int PercentOf(this int value, int total)
        {
            return (int)((value / (double)total) * 100);
        }

        /// <summary>
        /// Get percent of
        /// </summary>
        /// <param name="value"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static int PercentOf(this ulong value, ulong total)
        {
            return (int)((value / (double)total) * 100);
        }

        /// <summary>
        /// Get percent of
        /// </summary>
        /// <param name="value"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static int PercentOf(this long value, long total)
        {
            var v = value > 0 ? (ulong)value : 0;
            return total == 0 ? 0 : v.PercentOf((ulong)total);
        }
    }
}

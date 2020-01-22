using System;
using System.Collections.Generic;
using System.Linq;

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

        public static long NextLong(this Random random, long min, long max)
        {
            if (max <= min)
                throw new ArgumentOutOfRangeException("max", "max must be > min!");
            var uRange = (ulong)(max - min);
            ulong ulongRand;
            do
            {
                var buf = new byte[8];
                random.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);

            return (long)(ulongRand % uRange) + min;
        }

        /// <summary>
        /// Returns a random long from 0 (inclusive) to max (exclusive)
        /// </summary>
        /// <param name="random">The given random instance</param>
        /// <param name="max">The exclusive maximum bound.  Must be greater than 0</param>
        public static long NextLong(this Random random, long max)
        {
            return random.NextLong(0, max);
        }

        /// <summary>
        /// Returns a random long over all possible values of long (except long.MaxValue, similar to
        /// random.Next())
        /// </summary>
        /// <param name="random">The given random instance</param>
        public static long NextLong(this Random random)
        {
            return random.NextLong(long.MinValue, long.MaxValue);
        }

        /// <summary>
        /// Generate unique number
        /// </summary>
        /// <param name="excludeNumbers"></param>
        /// <returns></returns>
        public static long GenerateUniqueNumberThatNoIncludesNumbers(this IEnumerable<long> excludeNumbers)
        {
            var enumerated = excludeNumbers.ToList();
            var random = new Random();
            long number = 1;

            while (enumerated.Contains(number))
            {
                number = random.NextLong(1, long.MaxValue);
            }

            return number;
        }

        /// <summary>
        /// Is prime number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsPrime(this int number)
        {
            if ((number % 2) == 0)
            {
                return number == 2;
            }
            var sqrt = (int)Math.Sqrt(number);
            for (var t = 3; t <= sqrt; t += 2)
            {
                if (number % t == 0)
                {
                    return false;
                }
            }
            return number != 1;
        }
    }
}

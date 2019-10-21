using System;
using System.Diagnostics;

namespace GR.Core.Helpers
{
    public static class Arg
    {
        [DebuggerStepThrough]
        public static void NotNull<T>(T value, string name) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(name);
        }

        [DebuggerStepThrough]
        public static void NotNull<T>(T? value, string name) where T : struct
        {
            if (!value.HasValue)
                throw new ArgumentNullException(name);
        }

        [DebuggerStepThrough]
        public static void NotNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(name);
        }

        [DebuggerStepThrough]
        public static void InRange<T>(T value, T minValue, string name) where T : IComparable<T>
        {
            if (value.CompareTo(minValue) < 0)
                throw new ArgumentOutOfRangeException(name);
        }

        [DebuggerStepThrough]
        public static void InRange<T>(T value, T minValue, T maxValue, string name) where T : IComparable<T>
        {
            if (value.CompareTo(minValue) < 0 || value.CompareTo(maxValue) > 0)
                throw new ArgumentOutOfRangeException(name);
        }

        [DebuggerStepThrough]
        public static void LessThan<T>(T param, T value, string paramName) where T : struct, IComparable<T>
        {
            if (param.CompareTo(value) >= 0)
                throw new ArgumentOutOfRangeException(paramName);
        }

        [DebuggerStepThrough]
        public static void LessThanOrEqualTo<T>(T param, T value, string paramName) where T : struct, IComparable<T>
        {
            if (param.CompareTo(value) > 0)
                throw new ArgumentOutOfRangeException(paramName);
        }

        [DebuggerStepThrough]
        public static void GreaterThan<T>(T param, T value, string paramName) where T : struct, IComparable<T>
        {
            if (param.CompareTo(value) <= 0)
                throw new ArgumentOutOfRangeException(paramName);
        }

        [DebuggerStepThrough]
        public static void GreaterThanOrEqualTo<T>(T param, T value, string paramName) where T : struct, IComparable<T>
        {
            if (param.CompareTo(value) < 0)
                throw new ArgumentOutOfRangeException(paramName);
        }
    }
}

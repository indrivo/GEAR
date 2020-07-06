using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GR.Core.Helpers
{
    public static class ObjectIdentificationHelper
    {
        /// <summary>
        /// Check if object is list
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsList(this object o)
        {
            if (o == null) return false;

            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        /// <summary>
        /// Check if object id dictionary
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsDictionary(this object o)
        {
            if (o == null) return false;

            return o is IDictionary &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }

        /// <summary>
        /// Check if object is int type
        /// </summary>
        /// <param name="sVal"></param>
        /// <returns></returns>
        public static bool IsInt(this string sVal)
        {
            return sVal.Select(c => (int) c).All(iN => (iN <= 57) && (iN >= 48));
        }

        /// <summary>
        /// Check if is numeric
        /// </summary>
        /// <param name="sVal"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string sVal)
        {
            var isNum = double.TryParse(Convert.ToString(sVal), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out _);
            return isNum;
        }
    }
}

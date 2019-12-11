using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GR.Core.Helpers
{
    public static class ObjectIdentificationHelper
    {
        public static bool IsList(this object o)
        {
            if (o == null) return false;

            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public static bool IsDictionary(this object o)
        {
            if (o == null) return false;

            return o is IDictionary &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }

        public static bool IsInt(this string sVal)
        {
            return sVal.Select(c => (int) c).All(iN => (iN <= 57) && (iN >= 48));
        }

        public static bool IsNumeric(this string sVal)
        {
            var isNum = double.TryParse(Convert.ToString(sVal), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out _);
            return isNum;
        }
    }
}

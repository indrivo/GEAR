using System;
using GR.Core.Extensions;

namespace GR.Core.Helpers
{
    public static class TypeHelperExtensions
    {
        public static object GetPropertyValue(this object obj, string name)
        {
            return obj?.GetType()
                .GetProperty(name.Trim().FirstCharToUpper())
                ?.GetValue(obj, null);
        }


        public static string GetStringPropertyValue(this object obj, string name)
        {
            return obj?.GetType()
                .GetProperty(name.Trim().FirstCharToUpper())
                ?.GetValue(obj, null)?.ToString() ?? string.Empty;
        }
    }
}

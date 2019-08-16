using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace ST.Report.Abstractions.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>()
                           .Name;
        }

    }

    public static class Enum<T> where T : Enum, IConvertible
    {
        public static Dictionary<int, string> ToDictionary()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(e => Convert.ToInt32(e), e => e.GetDisplayName());
        }
    }
}

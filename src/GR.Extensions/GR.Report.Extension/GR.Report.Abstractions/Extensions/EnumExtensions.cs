using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace GR.Report.Abstractions.Extensions
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

        public static string GetDescription(this Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString())
                .First()
                .GetCustomAttribute<DescriptionAttribute>()
                .Description;
        }
    }

    public static class Enum<T> where T : Enum, IConvertible
    {
        public static Dictionary<T, string> ToDictionary(bool displayName = true)
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(e => e, e => displayName ? e.GetDisplayName() : e.ToString());
        }
    }
}

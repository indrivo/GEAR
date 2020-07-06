using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace GR.Core.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Get enum definition
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetEnumDefinition(this Type enumType)
        {
            if (!enumType.IsEnum) throw new Exception("Non valid enum");
            return Enum.GetNames(enumType).ToDictionary(x => (int)Enum.Parse(enumType, x), x => x);
        }

        /// <summary>
        /// Get enum member value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumMemberValue<T>(this T value) where T : struct, IConvertible
        {
            var element = typeof(T).GetTypeInfo().DeclaredMembers.SingleOrDefault(x => x.Name == value.ToString(CultureInfo.InvariantCulture));
            if ((object)element == null)
                return null;
            return element.GetCustomAttribute<EnumMemberAttribute>(false)?.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToEnumMemberValue<T>(this string str)
        {
            var enumType = typeof(T);
            foreach (var name in Enum.GetNames(enumType))
            {
                var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
                if (enumMemberAttribute.Value == str) return (T)Enum.Parse(enumType, name);
            }
            return default;
        }
    }
}

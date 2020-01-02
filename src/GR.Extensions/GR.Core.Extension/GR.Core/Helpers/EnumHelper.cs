using System;
using System.ComponentModel;

namespace GR.Core.Helpers
{
    public static class EnumHelper
    {
        /// <summary>
        /// Get enumerator description
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name == null) return null;

            var field = type.GetField(name);
            if (field == null) return null;

            var attr =
                Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attr?.Description;
        }
    }
}
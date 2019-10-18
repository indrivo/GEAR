using System;
using System.Collections.Generic;
using System.Linq;

namespace GR.Core.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Get enum definition
        /// </summary>
        /// <param name="enm"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetEnumDefinition(this Type enumType)
        {
            if (!enumType.IsEnum) throw new Exception("Non valid enum");
            return Enum.GetNames(enumType).ToDictionary(x => (int)Enum.Parse(enumType, x), x => x);
        }
    }
}

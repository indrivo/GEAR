using System;
using System.Collections.Generic;
using System.Linq;

namespace GR.DynamicEntityStorage.Abstractions.Helpers
{
    public static class DynamicTypeMapper
    {
        private static readonly Dictionary<Type, List<string>> MapperStorage = new Dictionary<Type, List<string>>
        {
            {typeof(string), new List<string> {"nvarchar"}},
            {typeof(int), new List<string> {"int", "int32"}},
            {typeof(char), new List<string> {"char"}},
            {typeof(bool), new List<string> {"bool"}},
            {typeof(Guid), new List<string> {"uniqueidentifier"}},
            {typeof(long), new List<string> {"bigint"}},
            {typeof(DateTime), new List<string> {"datetime", "date"}},
            {typeof(double), new List<string> {"decimal"}}
        };

        /// <summary>
        /// Get type from string definition
        /// </summary>
        /// <param name="stringType"></param>
        /// <returns></returns>
        public static Type GetTypeFromString(string stringType)
        {
            var typeMatch = MapperStorage.FirstOrDefault(x => x.Value.Contains(stringType));
            if (typeMatch.Equals(new KeyValuePair<Type, List<string>>()))
            {
                throw new InvalidCastException($"System does not have definition for {stringType}");
            }

            return typeMatch.Key;
        }
    }
}

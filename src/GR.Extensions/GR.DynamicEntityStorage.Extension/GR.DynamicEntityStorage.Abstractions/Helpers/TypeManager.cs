using System;
using System.Collections.Generic;
using GR.Core.Helpers;

namespace GR.DynamicEntityStorage.Abstractions.Helpers
{
    public static class TypeManager
    {
        /// <summary>
        /// Store types
        /// </summary>
        private static Dictionary<string, Dictionary<string, Type>> InMemoryStorageItems { get; set; }

        /// <summary>
        /// Try get type
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static ResultModel<Type> TryGet(string assemblyName, string schema)
        {
            var result = new ResultModel<Type>();
            if (InMemoryStorageItems == null)
            {
                InMemoryStorageItems = new Dictionary<string, Dictionary<string, Type>>();
            }

            if (!InMemoryStorageItems.ContainsKey(schema)) return result;
            if (!InMemoryStorageItems[schema].ContainsKey(assemblyName)) return result;
            var instanceType = InMemoryStorageItems[schema][assemblyName];
            result.IsSuccess = true;
            result.Result = instanceType;
            return result;
        }

        /// <summary>
        /// Register new type
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        public static void Register(string schema, string entity, Type type)
        {
            if (!InMemoryStorageItems.ContainsKey(schema))
            {
                InMemoryStorageItems.Add(schema, new Dictionary<string, Type>());
            }

            if (!InMemoryStorageItems[schema].ContainsKey(entity))
            {
                InMemoryStorageItems[schema].Add(entity, type);
            }
            else
            {
                InMemoryStorageItems[schema][entity] = type;
            }
        }

        /// <summary>
        /// Clear type in memory
        /// </summary>
        public static void Clear()
        {
            InMemoryStorageItems?.Clear();
        }
    }
}

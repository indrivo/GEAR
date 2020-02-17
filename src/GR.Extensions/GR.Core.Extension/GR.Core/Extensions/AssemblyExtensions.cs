using System;
using System.Collections.Generic;
using AutoMapper;

namespace GR.Core.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Get type 
        /// </summary>
        /// <param name="_"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static Type GetTypeFromAssembliesByClassName(this object _, string className)
        {
            Type type = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var aType in assembly.GetTypes())
                {
                    if (!aType.IsClass || aType.Name != className) continue;
                    type = aType;
                    break;
                }
            }

            return type;
        }

        /// <summary>
        /// Get AutoMapper profiles from all assemblies
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAutoMapperProfilesFromAllAssemblies(this object _)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var aType in assembly.GetTypes())
                {
                    if (aType.IsClass && !aType.IsAbstract && aType.IsSubclassOf(typeof(Profile)))
                        yield return aType;
                }
            }
        }
    }
}

using System;

namespace GR.Core.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Get type 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static Type GetTypeFromAssembliesByClassName(this object context, string className)
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
    }
}

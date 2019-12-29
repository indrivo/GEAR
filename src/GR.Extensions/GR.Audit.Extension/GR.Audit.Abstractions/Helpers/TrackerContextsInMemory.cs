using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GR.Core.Extensions;
using GR.Core.Helpers;

namespace GR.Audit.Abstractions.Helpers
{
    public static class TrackerContextsInMemory
    {
        /// <summary>
        /// Storage
        /// </summary>
        private static readonly ConcurrentDictionary<string, Type> Storage = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Type> GetAll() => Storage.ToDictionary(k => k.Key, v => v.Value);

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public static void Register(string name, Type type)
        {
            Arg.NotNullOrEmpty(name, nameof(Register));
            Arg.NotNull(type, nameof(Register));
            Storage.TryAdd(name, type);
        }

        /// <summary>
        /// Get context module
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public static ITrackerDbContext GetContextModule(string moduleName)
        {
            Arg.NotNullOrEmpty(moduleName, nameof(GetContextModule));
            var module = Storage.FirstOrDefault(x => x.Key.Equals(moduleName));
            if (module.IsNull()) return null;

            return (ITrackerDbContext)IoC.Resolve(module.Value);
        }
    }
}

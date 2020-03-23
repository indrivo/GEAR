using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace GR.Modules.Abstractions.Helpers
{
    public static class ModulesProvider
    {
        /// <summary>
        /// Configuration builder
        /// </summary>
        internal static IConfigurationBuilder ConfigurationBuilder { get; set; }

        /// <summary>
        /// Modules
        /// </summary>
        internal static List<IModule> Modules = new List<IModule>();

        /// <summary>
        /// Bind 
        /// </summary>
        public static void Bind(IConfigurationBuilder builder)
        {
            ConfigurationBuilder = builder;
            AppDomain.CurrentDomain.AssemblyLoad += (sender, args) =>
            {
                var newModules = ExtractModulesFromAssembly(args.LoadedAssembly);
                foreach (var conf in newModules)
                {
                    conf.ApplyBuilderConfiguration(ConfigurationBuilder);
                }
            };
        }

        /// <summary>
        /// Get providers
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IModule> GetProviders()
        {
            if (Modules.Any()) return Modules.ToList();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) ExtractModulesFromAssembly(assembly);

            return Modules.ToList();
        }

        /// <summary>
        /// Extract modules from assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static IEnumerable<IModule> ExtractModulesFromAssembly(Assembly assembly)
        {
            var modules = new List<IModule>();
            foreach (var aType in assembly.GetTypes())
            {
                if (!aType.IsClass || aType.IsAbstract || !aType.IsSubclassOf(typeof(GearModule))) continue;
                var instance = (IModule)Activator.CreateInstance(aType);
                modules.Add(instance);
            }
            if (modules.Count > 1) throw new Exception("Multiple module configurations detected in one module, " +
                                                       $"please remove others. Assembly: {assembly.GetName().Name}");
            if (modules.Any()) Modules.AddRange(modules);
            return modules;
        }
    }
}

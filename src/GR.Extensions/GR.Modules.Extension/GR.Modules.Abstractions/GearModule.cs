using System;
using Microsoft.Extensions.Configuration;

namespace GR.Modules.Abstractions
{
    public abstract class GearModule : IModule
    {
        /// <summary>
        /// Name
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// System name
        /// </summary>
        public string SystemName => GetType().Assembly.GetName().Name;

        /// <summary>
        /// Version
        /// </summary>
        public string Version => GetType().Assembly.GetName().Version.ToString();

        /// <summary>
        /// Configuration
        /// </summary>
        public abstract Action<IConfigurationBuilder> Configuration { get; }

        /// <summary>
        /// Apply configuration
        /// </summary>
        /// <param name="builder"></param>
        public void ApplyBuilderConfiguration(IConfigurationBuilder builder) => Configuration?.Invoke(builder);
    }
}

using System;
using System.IO;
using GR.Modules.Abstractions;
using Microsoft.Extensions.Configuration;

namespace GR.Localization
{
    public class Module : GearModule
    {
        /// <summary>
        /// Name
        /// </summary>
        public override string Name => "Localization module";

        /// <summary>
        /// Configuration
        /// </summary>
        public override Action<IConfigurationBuilder> Configuration => builder =>
        {
            var path = Path.Combine(AppContext.BaseDirectory, "translationSettings.json");
            builder.AddJsonFile(path, true, true);
        };
    }
}

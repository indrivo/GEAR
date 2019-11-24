using System.Runtime.InteropServices;

namespace GR.Core
{
    public class GearApplication
    {
        /// <summary>
        /// App host
        /// </summary>
        protected static dynamic GlobalAppHost { get; set; }

        /// <summary>
        /// Configuration
        /// </summary>
        protected static dynamic GlobalAppConfiguration { get; set; }

        /// <summary>
        /// Get app host
        /// </summary>
        /// <typeparam name="THost"></typeparam>
        /// <returns></returns>
        public static THost GetHost<THost>() => (THost)GlobalAppHost;

        /// <summary>
        /// App configuration
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <returns></returns>
        public static TConfiguration GetConfiguration<TConfiguration>() => (TConfiguration)GlobalAppConfiguration;

        public static bool IsHostedOnLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
}

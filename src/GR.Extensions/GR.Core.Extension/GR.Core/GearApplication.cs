using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Castle.Windsor;
using GR.Core.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Patterns;
using Microsoft.Extensions.Configuration;

namespace GR.Core
{
    /// <summary>
    /// Abstract Gear app for be extended on
    /// different platforms like web, mobile, desktop
    /// </summary>
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public abstract class GearApplication
    {
        /// <summary>
        /// App version
        /// </summary>
        public static string AppVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();

        /// <summary>
        /// Application name
        /// </summary>
        public static string ApplicationName = "Gear";

        /// <summary>
        /// Set app version
        /// </summary>
        /// <param name="version"></param>
        public static void SetAppVersion(string version)
        {
            AppVersion = version;
        }

        /// <summary>
        /// Set app version
        /// </summary>
        /// <param name="name"></param>
        public static void SetAppName(string name)
        {
            ApplicationName = name;
        }

        /// <summary>
        /// Check if 
        /// </summary>
        /// <returns></returns>
        public static bool IsDevelopment()
        {
            var parent = Directory.GetParent(AppContext.BaseDirectory);
            return parent.Name.StartsWith("netcoreapp");
        }

        public static SystemConfig SystemConfig => Singleton<SystemConfig, SystemConfig>.GetOrSetInstance(
            () => Task.FromResult(IoC.Resolve<IConfiguration>().GetSection("SystemConfig").Get<SystemConfig>()));

        /// <summary>
        /// Execute tasks in queue
        /// </summary>
        public static IBackgroundTaskQueue BackgroundTaskQueue =>
            Singleton<IBackgroundTaskQueue, IBackgroundTaskQueue>.GetOrSetInstance(() =>
                Task.FromResult(IoC.ResolveNonRequired<IBackgroundTaskQueue>()));

        /// <summary>
        /// Services container
        /// </summary>
        public static IWindsorContainer ServicesContainer => IoC.Container;

        /// <summary>
        /// Is configured
        /// </summary>
        protected static bool ConfigurationsApplied { get; set; }

        /// <summary>
        /// Is configured
        /// </summary>
        public static bool Configured => ConfigurationsApplied;

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

        /// <summary>
        /// Check platform host
        /// </summary>
        /// <returns></returns>
        public static bool IsHostedOnLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        /// <summary>
        /// Running project path
        /// </summary>
        public static string RunningProjectPath => AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));

        public static class AppState
        {
            public static bool InstallOnProgress { get; set; } = false;
            public static bool Installed { get; set; } = true;
        }
    }
}
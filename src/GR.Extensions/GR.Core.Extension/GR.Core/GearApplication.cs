using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        /// <summary>
        /// Retrieve system config
        /// </summary>
        public static SystemConfig SystemConfig => Singleton<SystemConfig, SystemConfig>.GetOrSetInstance(
            () => Task.FromResult(IoC.Resolve<IConfiguration>().GetSection("SystemConfig").Get<SystemConfig>()));

        /// <summary>
        /// Execute tasks in queue
        /// </summary>
        public static IBackgroundTaskQueue BackgroundTaskQueue =>
            Singleton<IBackgroundTaskQueue, IBackgroundTaskQueue>.GetOrSetInstance(() =>
                Task.FromResult(IoC.ResolveNonRequired<IBackgroundTaskQueue>()));

        /// <summary>
        /// Service provider
        /// </summary>
        public static IServiceProvider ServiceProvider =>
            Singleton<IServiceProvider, IServiceProvider>.GetOrSetInstance(() => Task.FromResult(IoC.Resolve<IServiceProvider>()));

        /// <summary>
        /// Services container
        /// </summary>
        public static IWindsorContainer ServicesContainer => IoC.Container;

        /// <summary>
        /// Is configured
        /// </summary>
        public static bool Configured
        {
            get
            {
                var configuration = IoC.Resolve<IConfiguration>();
                var isConfigured = configuration.GetValue<bool>("IsConfigured");
                return isConfigured;
            }
        }

        /// <summary>
        /// App host
        /// </summary>
        protected static IHost GlobalAppHost { get; set; }

        /// <summary>
        /// Configuration
        /// </summary>
        protected static dynamic GlobalAppConfiguration { get; set; }

        /// <summary>
        /// Get app host
        /// </summary>
        /// <returns></returns>
        public static IHost GetHost() => GlobalAppHost;

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
        /// Run in scope
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static Task<T> RunInScopeAsync<T, TService>(Func<TService, Task<T>> task)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<TService>();
                return task(service);
            }
        }

        /// <summary>
        /// Run in scope
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static T RunInScope<T, TService>(Func<TService, T> task)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<TService>();
                return task(service);
            }
        }

        /// <summary>
        /// Get assemblies
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("GR.")).ToList();
        }

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
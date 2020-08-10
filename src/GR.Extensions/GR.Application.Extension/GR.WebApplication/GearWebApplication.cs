using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GR.Core.Events;
using GR.Core.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Patterns;
using GR.Modules.Abstractions.Helpers;
using GR.WebApplication.Extensions;
using GR.WebApplication.Helpers;
using GR.WebApplication.Models;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace GR.WebApplication
{
    /// <summary>
    /// Gear app for run on web env
    /// </summary>
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class GearWebApplication : GearApplication
    {
        /// <summary>
        /// Application arguments
        /// </summary>
        public static GearApplicationArgs ApplicationArgs;

        /// <summary>
        /// Build web host
        /// </summary>
        protected static IHost GlobalWebHost;

        /// <summary>
        /// Get startup configuration
        /// </summary>
        protected static GearCoreStartup StartupConfiguration => Singleton<GearCoreStartup, GearCoreStartup>.GetOrSetInstance(
            () =>
            {
                var startupType = GlobalWebHost.GetTypeFromAssembliesByClassName("Startup");
                if (startupType == null) throw new Exception("Can't find a Startup class, please define one and inherit from GearCoreStartup class");
                var configuration = IoC.Resolve<IConfiguration>();
                var hostingEnvironment = IoC.Resolve<IWebHostEnvironment>();
                var instance = (GearCoreStartup)Activator.CreateInstance(startupType, configuration, hostingEnvironment);
                return Task.FromResult(instance);
            });

        /// <summary>
        /// Init migrations
        /// </summary>
        public static void InitModulesMigrations()
        {
            AppState.InstallOnProgress = true;

            StartupConfiguration.OnBeforeDatabaseMigrationsApply(GlobalWebHost)
                .Wait();

            SystemEvents.Database.MigrateAll(EventArgs.Empty);
        }

        /// <summary>
        /// Run application
        /// </summary>
        /// <param name="args"></param>
        /// <param name="gearApplicationArgs"></param>
        public static void Run<TStartUp>(string[] args, GearApplicationArgs gearApplicationArgs = null) where TStartUp : class
        {
            ApplicationArgs = gearApplicationArgs ?? new GearApplicationArgs();
            SystemEvents.RegisterEvents();
            BuildWebHost<TStartUp>(args).Run();
        }

        #region Helpers

        /// <summary>
        /// Build configuration
        /// </summary>
        /// <returns></returns>
        private static IConfigurationRoot BuildConfiguration()
        {
            InitAppSettingsFiles();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            GlobalAppConfiguration = config;
            return config;
        }

        /// <summary>
        /// Init app settings files
        /// </summary>
        [Conditional("DEBUG")]
        public static void InitAppSettingsFiles()
        {
            //System env
            var envPaths = new List<string>
            {
                GlobalResources.Environments.DEVELOPMENT, GlobalResources.Environments.STAGE, string.Empty
            };

            var fails = 0;

            foreach (var subPath in envPaths)
            {
                var pathBuilder = new StringBuilder("appsettings.");
                if (!subPath.IsNullOrEmpty()) pathBuilder.AppendFormat("{0}.", subPath);
                pathBuilder.Append("json");
                var appSettingsFilePath = Path.Combine(RunningProjectPath, pathBuilder.ToString());
                if (!File.Exists(appSettingsFilePath)) using (var _ = File.Create(appSettingsFilePath)) { }
                var settings = JsonParser.ReadObjectDataFromJsonFile<AppSettingsModel.RootObject>(appSettingsFilePath);
                if (settings != null) continue;
                fails++;
                var baseSettings = JsonConvert.SerializeObject(new AppSettingsModel.RootObject(), Formatting.Indented);
                File.WriteAllText(appSettingsFilePath, baseSettings);
            }

            if (fails > 0) throw new Exception("Please restart the application to configure it correctly, " +
                                               "we have created a template with which you can configure it in appsettings.{EnvName}.json");
        }

        /// <summary>
        /// Build web host
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IHost BuildWebHost<TStartUp>(string[] args) where TStartUp : class
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<TStartUp>();
                webBuilder.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                webBuilder.UseConfiguration(BuildConfiguration());
                webBuilder.CaptureStartupErrors(true);
                webBuilder.ConfigureAppConfiguration((hostingContext, conf) => { ModulesProvider.Bind(conf); });
                if (ApplicationArgs.UseKestrel)
                {
                    webBuilder.UseKestrel(options => options.ConfigureEndpoints());
                }
            });


            if (GlobalWebHost != null) return GlobalWebHost;

            GlobalAppHost = GlobalWebHost = builder.Build();
            return GlobalWebHost;
        }

        #endregion
    }
}
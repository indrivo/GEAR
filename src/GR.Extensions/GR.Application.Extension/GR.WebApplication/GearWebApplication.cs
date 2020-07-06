using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using GR.Core.Events;
using GR.Core.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers.Global;
using GR.Entities.Abstractions;
using GR.Identity.Abstractions;
using GR.Logger.Extensions;
using GR.Modules.Abstractions.Helpers;
using GR.UI.Menu.Abstractions;
using GR.WebApplication.Extensions;
using GR.WebApplication.Models;
using Microsoft.AspNetCore;
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
        protected static IWebHost GlobalWebHost;

        /// <summary>
        /// Get settings
        /// </summary>
        public static AppSettingsModel.RootObject Settings(IHostingEnvironment hostingEnvironment)
            => JsonParser.ReadObjectDataFromJsonFile<AppSettingsModel.RootObject>(
                ResourceProvider.AppSettingsFilepath(hostingEnvironment));

        /// <summary>
        /// Init migrations
        /// </summary>
        public static void InitModulesMigrations() => Migrate();

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

        /// <summary>
        /// Is system configured
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <returns></returns>
        public static bool IsConfigured(IHostingEnvironment hostingEnvironment)
        {
            var settings = Settings(hostingEnvironment);
            ConfigurationsApplied = settings != null && settings.IsConfigured;
            return ConfigurationsApplied;
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
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
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
        private static IWebHost BuildWebHost<TStartUp>(string[] args) where TStartUp : class
        {
            if (GlobalWebHost != null) return GlobalWebHost;
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .UseConfiguration(BuildConfiguration())
                .RegisterGearLoggingProviders()
                .CaptureStartupErrors(true)
                .UseStartup<TStartUp>()
                .ConfigureAppConfiguration((hostingContext, conf) => { ModulesProvider.Bind(conf); });

            if (ApplicationArgs.UseKestrel)
            {
                builder.UseKestrel(options => options.ConfigureEndpoints());
            }

            GlobalAppHost = GlobalWebHost = builder.Build();
            return GlobalWebHost;
        }

        /// <summary>
        /// Migrate Web host extension
        /// </summary>
        /// <returns></returns>
        private static void Migrate()
        {
            AppState.InstallOnProgress = true;

            GlobalWebHost?
                .MigrateAbstractDbContext<IEntityContext>()
                .MigrateAbstractDbContext<IIdentityContext>()
                .MigrateAbstractDbContext<IMenuDbContext>();

            SystemEvents.Database.MigrateAll(EventArgs.Empty);
        }
        #endregion
    }
}
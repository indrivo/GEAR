using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using GR.Core.Events;
using GR.Core.Helpers;
using GR.Identity.Data;
using GR.Identity.IdentityServer4;
using GR.Identity.IdentityServer4.Seeders;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Events.EventArgs.Database;
using GR.Core.Extensions;
using GR.Core.Helpers.Global;
using GR.Entities.Data;
using GR.Logger.Extensions;
using GR.UI.Menu.Data;
using GR.WebApplication.Helpers;
using GR.WebApplication.Models;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace GR.WebApplication
{
    [Author(Authors.LUPEI_NICOLAE, 1.1, "Gear app for run on web env")]
    public class GearWebApplication : GearApplication
    {
        /// <summary>
        /// Build web host
        /// </summary>
        protected static IHost GlobalWebHost;

        /// <summary>
        /// Get settings
        /// </summary>
        public static AppSettingsModel.RootObject Settings(IHostEnvironment hostingEnvironment)
            => JsonParser.ReadObjectDataFromJsonFile<AppSettingsModel.RootObject>(
                ResourceProvider.AppSettingsFilepath(hostingEnvironment));

        /// <summary>
        /// Migrate and run
        /// </summary>
        public static void MigrateAndRun() => Migrate().Run();

        /// <summary>
        /// Init migrations
        /// </summary>
        public static void InitModulesMigrations() => Migrate();

        /// <summary>
        /// Migrate Web host extension
        /// </summary>
        /// <returns></returns>
        private static IHost Migrate()
        {
            AppState.InstallOnProgress = true;

            GlobalWebHost?
                .MigrateDbContext<EntitiesDbContext>()
                .MigrateDbContext<ApplicationDbContext>()
                .MigrateDbContext<MenuDbContext>()
                .MigrateDbContext<PersistedGrantDbContext>()
                .MigrateDbContext<ConfigurationDbContext>((context, services) =>
                {
                    var config = services.GetService<IConfiguration>();
                    var env = services.GetService<IWebHostEnvironment>();
                    var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();
                    var configurator = new IdentityServer4Configurator();
                    IdentityServerConfigDbSeeder.SeedAsync(configurator, context, applicationDbContext, config, env)
                        .Wait();
                });

            SystemEvents.Database.Migrate(new DatabaseMigrateEventArgs());

            return GlobalWebHost;
        }

        /// <summary>
        /// Run application
        /// </summary>
        /// <param name="args"></param>
        public static void Run<TStartup>(string[] args) where TStartup : GearCoreStartup
        {
            SystemEvents.RegisterEvents();
            BuildWebHost<TStartup>(args).Run();
        }

        /// <summary>
        /// Is system configured
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <returns></returns>
        public static bool IsConfigured(IHostEnvironment hostingEnvironment)
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
            InitAppsettingsFiles();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("fileSettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            GlobalAppConfiguration = config;
            return config;
        }

        /// <summary>
        /// Init appsettings files
        /// </summary>
        [Conditional("DEBUG")]
        public static void InitAppsettingsFiles()
        {
            //System env
            var envPaths = new List<string>
            {
                "Development", "Stage", string.Empty
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
        private static IHost BuildWebHost<TStartup>(string[] args) where TStartup : GearCoreStartup
        {
            GlobalAppHost = GlobalWebHost = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>();
                    webBuilder.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                    webBuilder.UseConfiguration(BuildConfiguration());
                    webBuilder.RegisterGearLoggingProviders();
                    webBuilder.CaptureStartupErrors(true);
                })
                .ConfigureAppConfiguration((hostingContext, conf) =>
                {
                    var path = Path.Combine(AppContext.BaseDirectory, "translationSettings.json");
                    conf.AddJsonFile(path, true, true);
                })
                .Build();

            return GlobalWebHost;
        }

        #endregion
    }
}
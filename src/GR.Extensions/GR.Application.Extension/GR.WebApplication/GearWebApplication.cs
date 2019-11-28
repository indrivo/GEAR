using System;
using System.IO;
using GR.Core.Events;
using GR.Core.Helpers;
using GR.Identity.Data;
using GR.Identity.IdentityServer4;
using GR.Identity.IdentityServer4.Seeders;
using GR.Identity.Seeders;
using GR.WebApplication.Extensions;
using GR.WebApplication.InstallerModels;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GR.Core;
using GR.Core.Extensions;
using Microsoft.AspNetCore;

namespace GR.WebApplication
{
    public class GearWebApplication : GearApplication
    {
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
        /// Migrate and run
        /// </summary>
        public static void MigrateAndRun() => Migrate().Run();

        /// <summary>
        /// Init migrations
        /// </summary>
        public static void InitMigrations() => Migrate();

        /// <summary>
        /// Migrate Web host extension
        /// </summary>
        /// <returns></returns>
        private static IWebHost Migrate()
        {
            GlobalWebHost?.MigrateDbContext<PersistedGrantDbContext>()
                .MigrateDbContext<ApplicationDbContext>((context, services) =>
                {
                    new ApplicationDbContextSeed()
                        .SeedAsync(context, services)
                        .Wait();
                })
                .MigrateDbContext<ConfigurationDbContext>((context, services) =>
                {
                    var config = services.GetService<IConfiguration>();
                    var env = services.GetService<IHostingEnvironment>();
                    var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();
                    var configurator = new IdentityServer4Configurator();
                    IdentityServerConfigDbSeeder.SeedAsync(configurator, context, applicationDbContext, config, env)
                        .Wait();
                });

            return GlobalWebHost;
        }

        /// <summary>
        /// Run application
        /// </summary>
        /// <param name="args"></param>
        public static void Run<TStartUp>(string[] args) where TStartUp : class
        {
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
        /// Build web host
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IWebHost BuildWebHost<TStartUp>(string[] args) where TStartUp : class
        {
            GlobalAppHost = GlobalWebHost = WebHost.CreateDefaultBuilder(args)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .UseConfiguration(BuildConfiguration())
                .StartLogging()
                .CaptureStartupErrors(true)
                .UseStartup<TStartUp>()
                .ConfigureAppConfiguration((hostingContext, conf) =>
                {
                    var path = Path.Combine(AppContext.BaseDirectory, "translationSettings.json");
                    conf.AddJsonFile(path, optional: true, reloadOnChange: true);
                })
                .UseSentry()
                .Build();

            return GlobalWebHost;
        }

        #endregion
    }
}
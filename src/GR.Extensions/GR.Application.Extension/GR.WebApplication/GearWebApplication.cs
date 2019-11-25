using System;
using System.IO;
using System.Threading.Tasks;
using GR.Cache.Abstractions;
using GR.Core.Events;
using GR.Core.Helpers;
using GR.DynamicEntityStorage.Abstractions;
using GR.DynamicEntityStorage.Abstractions.Seeders;
using GR.Entities.Abstractions;
using GR.Entities.Data;
using GR.Identity.Data;
using GR.Identity.IdentityServer4;
using GR.Identity.IdentityServer4.Seeders;
using GR.Identity.Permissions.Abstractions;
using GR.Identity.Seeders;
using GR.Notifications.Abstractions.Seeders;
using GR.PageRender.Abstractions.Helpers;
using GR.Report.Dynamic.Data;
using GR.WebApplication.Extensions;
using GR.WebApplication.InstallerModels;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GR.Core;
using GR.Core.Events.EventArgs.Database;
using GR.Core.Extensions;
using GR.Forms.Data;
using GR.PageRender.Data;
using GR.Procesess.Data;

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
            GlobalWebHost?.MigrateDbContext<EntitiesDbContext>((context, services) =>
                {
                    EntitiesDbContextSeeder<EntitiesDbContext>.SeedAsync(context, GearSettings.TenantId).Wait();
                })
                .MigrateDbContext<FormDbContext>((context, services) =>
                {
                    FormDbContextSeeder<FormDbContext>.SeedAsync(context, GearSettings.TenantId).Wait();
                })
                .MigrateDbContext<ProcessesDbContext>()
                .MigrateDbContext<DynamicPagesDbContext>((context, services) =>
                {
                    DynamicPagesDbContextSeeder<DynamicPagesDbContext>.SeedAsync(context, GearSettings.TenantId)
                        .Wait();
                })
                .MigrateDbContext<PersistedGrantDbContext>()
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
            SystemEvents.Database.Migrate(new DatabaseMigrateEventArgs());
            SystemEvents.Database.MigrateComplete(new DatabaseMigrateEventArgs());
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
            return settings != null && settings.IsConfigured;
        }

        /// <summary>
        /// Create dynamic tables
        /// </summary>
        /// <param name="tenantId"></param>
        public static async Task SyncDefaultEntityFrameWorkEntities(Guid tenantId)
        {
            //Seed EntityFrameWork entities
            var entities = IoC.Resolve<ITablesService>().GetEntitiesFromDbContexts(typeof(ApplicationDbContext),
                typeof(EntitiesDbContext), typeof(FormDbContext), typeof(DynamicReportDbContext));

            foreach (var ent in entities)
            {
                if (!await IoC.Resolve<EntitiesDbContext>().Table
                    .AnyAsync(s => s.Name == ent.Name && s.TenantId == tenantId))
                {
                    await IoC.Resolve<EntitySynchronizer>().SynchronizeEntities(ent, tenantId, ent.Schema);
                }
            }
        }

        /// <summary>
        /// Seed dynamic data 
        /// </summary>
        /// <returns></returns>
        public static async Task SeedDynamicDataAsync()
        {
            await Task.WhenAll(NotificationManager.SeedNotificationTypesAsync(),
                MenuManager.SyncMenuItemsAsync(),
                PageManager.SyncWebPagesAsync(),
                TemplateManager.SeedAsync(),
                NomenclatureManager.SyncNomenclaturesAsync());
        }

        /// <summary>
        /// On application start
        /// </summary>
        /// <param name="app"></param>
        public static async void ApplicationStarted(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var env = serviceScope.ServiceProvider.GetService<IHostingEnvironment>();
                var service = serviceScope.ServiceProvider.GetService<IDynamicService>();
                var context = serviceScope.ServiceProvider.GetService<DynamicPagesDbContext>();
                var cacheService = serviceScope.ServiceProvider.GetService<ICacheService>();
                if (!IsConfigured(env)) return;
                await DynamicPagesDbContextSeeder<DynamicPagesDbContext>.SeedAsync(context, GearSettings.TenantId);
                //Run only if application is configured
                var permissionService = serviceScope.ServiceProvider.GetService<IPermissionService>();
                cacheService.FlushAll();
                await permissionService.RefreshCache();
                await service.RegisterInMemoryDynamicTypesAsync();
            }
        }

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
            GlobalAppHost = GlobalWebHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
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
    }
}
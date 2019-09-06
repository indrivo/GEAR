using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sentry;
using ST.Application.Extensions;
using ST.Application.InstallerModels;
using ST.Cache.Abstractions;
using ST.Core.Events;
using ST.Core.Helpers;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Abstractions.Seeders;
using ST.Entities.Abstractions;
using ST.Entities.Data;
using ST.Forms.Data;
using ST.Identity.Data;
using ST.Identity.IdentityServer4;
using ST.Identity.IdentityServer4.Seeders;
using ST.Identity.Permissions.Abstractions;
using ST.Identity.Seeders;
using ST.Notifications.Abstractions.Seeders;
using ST.PageRender.Data;
using ST.PageRender.Razor.Helpers;
using ST.Procesess.Data;
using ST.Report.Dynamic.Data;

namespace ST.Application
{
    public static class CoreApp
    {
        /// <summary>
        /// Build web host
        /// </summary>
        private static IWebHost _webHost;

        /// <summary>
        /// Get settings
        /// </summary>
        public static AppSettingsModel.RootObject Settings(IHostingEnvironment hostingEnvironment)
            => JsonParser.ReadObjectDataFromJsonFile<AppSettingsModel.RootObject>(
                ResourceProvider.AppSettingsFilepath(hostingEnvironment));

        /// <summary>
        /// Run
        /// </summary>
        public static void MigrateAndRun() => _webHost
            .Migrate()
            .Run();

        /// <summary>
        /// Init migrations
        /// </summary>
        public static void InitMigrations() => _webHost.Migrate();

        /// <summary>
        /// Migrate Web host extension
        /// </summary>
        /// <param name="webHost"></param>
        /// <returns></returns>
        private static IWebHost Migrate(this IWebHost webHost)
        {
            webHost.MigrateDbContext<EntitiesDbContext>((context, services) =>
                {
                    EntitiesDbContextSeeder<EntitiesDbContext>.SeedAsync(context, Core.Settings.TenantId).Wait();
                })
                .MigrateDbContext<FormDbContext>((context, services) =>
                {
                    FormDbContextSeeder<FormDbContext>.SeedAsync(context, Core.Settings.TenantId).Wait();
                })
                .MigrateDbContext<ProcessesDbContext>()
                .MigrateDbContext<DynamicPagesDbContext>((context, services) =>
                {
                    DynamicPagesDbContextSeeder<DynamicPagesDbContext>.SeedAsync(context, Core.Settings.TenantId)
                        .Wait();
                })
                .MigrateDbContext<DynamicReportDbContext>()
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

            return webHost;
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
        public static async void OnApplicationStarted(IApplicationBuilder app)
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
                await DynamicPagesDbContextSeeder<DynamicPagesDbContext>.SeedAsync(context, Core.Settings.TenantId);
                //Run only if application is configured
                var permissionService = serviceScope.ServiceProvider.GetService<IPermissionService>();
                cacheService.FlushAll();
                await permissionService.RefreshCache();
                await service.RegisterInMemoryDynamicTypesAsync();
            }
        }

        /// <summary>
        /// Return true if is 
        /// </summary>
        /// <returns></returns>
        public static bool IsHostedOnLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        /// <summary>
        /// Build web host
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IWebHost BuildWebHost<TStartUp>(string[] args) where TStartUp : class
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("translationSettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("fileSettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            _webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .UseConfiguration(config)
                .StartLogging()
                .CaptureStartupErrors(true)
                .UseStartup<TStartUp>()
                .UseSentry()
                .Build();

            return _webHost;
        }
    }
}
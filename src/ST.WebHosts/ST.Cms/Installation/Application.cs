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
using ST.Cache.Abstractions;
using ST.Configuration.Seed;
using ST.Core.Helpers;
using ST.DynamicEntityStorage.Abstractions;
using ST.Entities.Data;
using ST.Identity.Abstractions;
using ST.Identity.Data;
using ST.PageRender.Razor.Helpers;
using ST.Procesess.Data;
using ST.Cms.Extensions;
using ST.Cms.ViewModels.InstallerModels;
using ST.Entities.Abstractions;
using ST.Entities.Abstractions.Query;
using ST.Entities.EntityBuilder.Postgres;
using ST.Entities.EntityBuilder.Postgres.Controls.Query;
using ST.Forms.Data;
using ST.PageRender.Data;
using ST.Report.Dynamic.Data;

namespace ST.Cms.Installation
{
	public static class Application
	{
		/// <summary>
		/// Get settings
		/// </summary>
		public static AppSettingsModel.RootObject Settings(IHostingEnvironment hostingEnvironment)
			=> JsonParser.ReadObjectDataFromJsonFile<AppSettingsModel.RootObject>(ResourceProvider.AppSettingsFilepath(hostingEnvironment));


		/// <summary>
		/// Run
		/// </summary>
		/// <param name="args"></param>
		internal static void MigrateAndRun(string[] args)
		{
			BuildWebHost(args)
				.Migrate()
				.Run();
		}

		/// <summary>
		/// Init migrations
		/// </summary>
		/// <param name="args"></param>
		public static void InitMigrations(string[] args)
		{
			BuildWebHost(args)
				.Migrate();
		}

		/// <summary>
		/// Migrate Web host extension
		/// </summary>
		/// <param name="webHost"></param>
		/// <returns></returns>
		private static IWebHost Migrate(this IWebHost webHost)
		{
			webHost.MigrateDbContext<EntitiesDbContext>((context, services) =>
				   {
					   EntitiesDbContextSeed<EntitiesDbContext>.SeedAsync(context, Core.Settings.TenantId).Wait();
				   })
				.MigrateDbContext<FormDbContext>((context, services) =>
				   {
					   FormDbContextSeed<FormDbContext>.SeedAsync(context, Core.Settings.TenantId).Wait();
				   })
				.MigrateDbContext<ProcessesDbContext>()
				.MigrateDbContext<DynamicPagesDbContext>()
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
					IdentityServerConfigDbSeed.SeedAsync(context, applicationDbContext, config, env)
						.Wait();
				});

			return webHost;
		}

		/// <summary>
		/// Run application
		/// </summary>
		/// <param name="args"></param>
		public static void Run(string[] args)
		{
			BuildWebHost(args).Run();
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
			var entities = IoC.Resolve<ITablesService>().GetEntitiesFromDbContexts(typeof(ApplicationDbContext), typeof(EntitiesDbContext), typeof(FormDbContext), typeof(DynamicReportDbContext));

			foreach (var ent in entities)
			{
				if (!await IoC.Resolve<EntitiesDbContext>().Table.AnyAsync(s => s.Name == ent.Name && s.TenantId == tenantId))
				{
					IoC.Resolve<EntitySynchronizer>().SynchronizeEntities(ent, tenantId, ent.Schema);
				}
			}
		}

		/// <summary>
		/// Seed dynamic data 
		/// </summary>
		/// <returns></returns>
		public static async Task SeedDynamicDataAsync()
		{
			//Seed notifications types
			await NotificationManager.SeedNotificationTypesAsync();

			//Sync default menus
			await MenuManager.SyncMenuItemsAsync();

			//Sync web pages
			await PageManager.SyncWebPagesAsync();

			//Sync templates
			await TemplateManager.SeedAsync();

			//Sync nomenclatures
			await NomenclatureManager.SyncNomenclaturesAsync();
		}

		/// <summary>
		/// On application start
		/// </summary>
		/// <param name="app"></param>
		internal static async void OnApplicationStarted(IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices
				.GetRequiredService<IServiceScopeFactory>()
				.CreateScope())
			{
				var env = serviceScope.ServiceProvider.GetService<IHostingEnvironment>();
				var service = serviceScope.ServiceProvider.GetService<IDynamicService>();
				var cacheService = serviceScope.ServiceProvider.GetService<ICacheService>();

				var isConfigured = IsConfigured(env);

				if (!isConfigured) return;
				
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
		internal static bool IsHostedOnLinux()
		{
			return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
		}

		/// <summary>
		/// Build web host
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private static IWebHost BuildWebHost(string[] args)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddEnvironmentVariables()
				.AddJsonFile("appsettings.json", optional: false)
				.Build();

			return Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
				.UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
				.UseConfiguration(config)
				.StartLogging()
				.CaptureStartupErrors(true)
				.UseStartup<Startup>()
				.Build();
		}
	}
}

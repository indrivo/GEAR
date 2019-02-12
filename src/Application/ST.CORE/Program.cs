using System;
using System.IO;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ST.CORE.Extensions;
using ST.CORE.Extensions.Installer;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Entities.Services;
using ST.Identity.Data;
using ST.Procesess.Data;

namespace ST.CORE
{
	public static class Program
	{
		/// <summary>
		/// Main
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			BuildWebHost(args)
				.MigrateDbContext<EntitiesDbContext>((context, services) =>
			   {
				   var conf = services.GetService<IConfiguration>();
				   EntitiesDbContextSeed.SeedAsync(context, conf)
					   .Wait();

				   var entity =
					   EntitiesDbContextSeed.ReadData(Path.Combine(AppContext.BaseDirectory, "SysEntities.json"));

				   if (entity.SynchronizeTableViewModels != null)
				   {
					   foreach (var ent in entity.SynchronizeTableViewModels)
					   {
						   if (EntityStorage.DynamicEntities.FirstOrDefault(x => x.Name.Equals(ent.Name)) == null)
						   {
							   ent.AddEntityToStorage();
						   }
						   if (!context.Table.Any(s => s.Name == ent.Name))
						   {
							   IoC.Resolve<EntitySynchronizer>().SynchronizeEntities(ent);
						   }
					   }
				   }

				   //Seed EntityFrameWork entities
				   var entities = TablesService.GetEntitiesFromDbContexts(typeof(ApplicationDbContext), typeof(EntitiesDbContext));

				   foreach (var ent in entities)
				   {
					   if (EntityStorage.DynamicEntities.FirstOrDefault(x => x.Name.Equals(ent.Name)) == null)
					   {
						   ent.AddEntityToStorage();
					   }

					   if (!context.Table.Any(s => s.Name == ent.Name))
					   {
						   IoC.Resolve<EntitySynchronizer>().SynchronizeEntities(ent);
					   }
				   }


				   var entityProfiles =
					   EntitiesDbContextSeed.ReadData(Path.Combine(AppContext.BaseDirectory, "ProfileEntities.json"));

				   if (entityProfiles.SynchronizeTableViewModels == null) return;
				   {
					   foreach (var ent in entityProfiles.SynchronizeTableViewModels)
					   {
						   if (EntityStorage.DynamicEntities.FirstOrDefault(x => x.Name.Equals(ent.Name)) == null)
						   {
							   ent.AddEntityToStorage();
						   }

						   if (!context.Table.Any(s => s.Name == ent.Name))
						   {
							   IoC.Resolve<EntitySynchronizer>().SynchronizeEntities(ent);
						   }
					   }
				   }

				   context.SyncWebPages(services);
			   })
				.MigrateDbContext<ProcessesDbContext>()
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
				})
				.Run();
		}

		/// <summary>
		/// Build web host
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public static IWebHost BuildWebHost(string[] args)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddEnvironmentVariables()
				.AddJsonFile("appsettings.json", optional: false)
				.Build();

			return WebHost.CreateDefaultBuilder(args)
				.UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
				.UseConfiguration(config)
				.CaptureStartupErrors(true)
				.UseStartup<Startup>()
				.StartLogging()
				.Build();
		}
	}
}
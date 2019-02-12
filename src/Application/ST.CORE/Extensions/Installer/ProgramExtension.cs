using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ST.Entities.Data;
using ST.Identity.Data;
using ST.Procesess.Data;

namespace ST.CORE.Extensions.Installer
{
	public static class ProgramExtension
	{
		public static void AddMigrations()
		{
			var args = new string[0];
			Program.BuildWebHost(args)
				.MigrateDbContext<EntitiesDbContext>()
				.MigrateDbContext<ProcessesDbContext>()
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
					var applicationDbContext = services.GetService<ApplicationDbContext>(); 
					IdentityServerConfigDbSeed.SeedAsync(context, applicationDbContext, config, env)
						.Wait();
				})
				.MigrateDbContext<EntitiesDbContext>((context, services) =>
				{
					var conf = services.GetService<IConfiguration>();
					EntitiesDbContextSeed.SeedAsync(context, conf)
						.Wait();
				});
		}

		public static void CreateDynamicTables()
		{
			var entitiesList = new List<EntitiesDbContextSeed.SeedEntity>
			{
				EntitiesDbContextSeed.ReadData(Path.Combine(AppContext.BaseDirectory, "SysEntities.json")),
				EntitiesDbContextSeed.ReadData(Path.Combine(AppContext.BaseDirectory, "ProfileEntities.json"))
			};

			foreach (var item in entitiesList)
			{
				if (item.SynchronizeTableViewModels == null) continue;
				foreach (var ent in item.SynchronizeTableViewModels)
				{
					if (!IoC.Resolve<EntitiesDbContext>().Table.Any(s => s.Name == ent.Name))
					{
						IoC.Resolve<EntitySynchronizer>().SynchronizeEntities(ent);
					}
				}
			}
		}
	}
}
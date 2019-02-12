using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ST.Identity.Configuration;
using ST.Identity.Data.Permissions;

namespace ST.Identity.Data
{
    public static class IdentityServerConfigDbSeed
    {
        public static async Task SeedAsync(ConfigurationDbContext context, ApplicationDbContext applicationDbContext,
            IConfiguration configuration, IHostingEnvironment env)
        {
            var clientUrls = new Dictionary<string, string>
            {
                ["CORE"] = GetClientUrl(env, configuration, "CORE"),
                ["BPMApi"] = GetClientUrl(env, configuration, "BPMApi")
            };

            //Seed clients
            if (!context.Clients.Any())
            {
                try
                {
                    var clients = Config.GetClients(clientUrls).Select(x => x.ToEntity()).GetSeedClients(context);
                    await context.Clients.AddRangeAsync(clients);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            //Seed Identity resources
            if (!context.IdentityResources.Any())
            {
                var resources = Config.GetResources().Select(x => x.ToEntity()).GetSeedResources(context);
                await context.IdentityResources.AddRangeAsync(resources);

                await context.SaveChangesAsync();
            }

            //Seed api resources
            if (!context.ApiResources.Any())
            {
                var apiResources = Config.GetApis().Select(x => x.ToEntity()).GetSeedApiResources(context);
                context.ApiResources.AddRange(apiResources);
                await context.SaveChangesAsync();
            }

            //Seed permmisions
            if (!applicationDbContext.Permissions.Any())
            {
                var clients = context.Clients.ToList();
                var permissionsList = (from item in PermissionsConstants.PermissionsList()
                                       let permissionConfig = item.Split("_")
                                       where permissionConfig.Length == 2
                                       let client = clients.FirstOrDefault(x => x.ClientId.ToLower().Equals(permissionConfig[0].ToLower()))
                                       select new Permission
                                       {
                                           Author = "System",
                                           Created = DateTime.Now,
                                           PermissionKey = item,
                                           PermissionName = permissionConfig[1],
                                           ClientId = client?.Id ?? 0,
                                           Description = $"Permission for module {permissionConfig[0]}",
                                           Changed = DateTime.Now
                                       }).ToList();

                try
                {
                    await applicationDbContext.Permissions.AddRangeAsync(permissionsList);
                    await applicationDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                var baseDirectory = AppContext.BaseDirectory;
                var entity = ApplicationDbContextSeed.ReadData(Path.Combine(baseDirectory, "IdentityConfiguration.json"));

                //Set core permissions for roles
                var coreClientId = clients.FirstOrDefault(y => y.ClientId.Equals("core"))?.Id;
                var corePermissions = applicationDbContext.Permissions.Where(x => x.ClientId.Equals(coreClientId));
                if (corePermissions.Any())
                {
                    await applicationDbContext.Roles.Where(x => x.ClientId.Equals(coreClientId))
                        .ForEachAsync(x =>
                        {
                            applicationDbContext.RolePermissions.AddRangeAsync(corePermissions.Select(o =>
                                new RolePermission
                                {
                                    RoleId = x.Id,
                                    PermissionId = o.Id,
                                    PermissionCode = o.PermissionKey
                                }));
                        });
                    await applicationDbContext.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Get uri of client from app settings;
        /// </summary>
        /// <param name="env"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static string GetClientUrl(IHostingEnvironment env, IConfiguration configuration, string sectionName)
        {
            var configs = configuration.GetSection("WebClients");
            var clientSection = configs.GetSection(sectionName);
            var identifiedSection = env.IsDevelopment() ? clientSection.GetSection("Dev") :
                env.IsEnvironment("Stage") ? clientSection.GetSection("Stage") : clientSection.GetSection("Prod");
            var uri = string.Empty;
            if (identifiedSection != null)
            {
                uri = identifiedSection.GetChildren().FirstOrDefault()?.Value;
            }

            return uri;
        }
    }
}
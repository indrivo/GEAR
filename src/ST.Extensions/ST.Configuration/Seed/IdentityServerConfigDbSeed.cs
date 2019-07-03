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
using ST.Configuration.Configuration;
using ST.Identity.Data.Permissions;
using ST.Identity.Data;

namespace ST.Configuration.Seed
{
    public static class IdentityServerConfigDbSeeder
    {
        public static async Task SeedAsync(ConfigurationDbContext context, ApplicationDbContext applicationDbContext,
            IConfiguration configuration, IHostingEnvironment env)
        {
            var clientUrls = new Dictionary<string, string>
            {
                ["CORE"] = GetClientUrl(env, configuration, "CORE"),
                ["BPMApi"] = GetClientUrl(env, configuration, "BPMApi"),
                ["eCommerce"] = GetClientUrl(env, configuration, "eCommerce")
            };

            //Seed clients
            if (!context.Clients.Any())
            {
                try
                {
                    var clients = Config.GetClients(clientUrls).GetSeedClients(context);
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
                var resources = Config.GetResources().GetSeedResources(context);
                await context.IdentityResources.AddRangeAsync(resources);

                await context.SaveChangesAsync();
            }

            //Seed api resources
            if (!context.ApiResources.Any())
            {
                var apiResources = Config.GetApis().GetSeedApiResources(context);
                context.ApiResources.AddRange(apiResources);
                await context.SaveChangesAsync();
            }

            //Seed permissions
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
                        .ForEachAsync(async x =>
                        {
                            var configuredPermissions =
                                entity.ApplicationRoles.FirstOrDefault(y => y.Name.Equals(x.Name));
                            if (configuredPermissions == null) return;

                            if (configuredPermissions.Permissions == null || !configuredPermissions.Permissions.Any())
                            {
                                await applicationDbContext.RolePermissions.AddRangeAsync(corePermissions.Select(o =>
                                    new RolePermission
                                    {
                                        RoleId = x.Id,
                                        PermissionId = o.Id,
                                        PermissionCode = o.PermissionKey
                                    }));
                            }
                            else
                            {
                                await applicationDbContext.RolePermissions.AddRangeAsync(configuredPermissions.Permissions.Select(
                                    o =>
                                    {
                                        var permission = corePermissions.FirstOrDefault(c =>
                                            c.PermissionKey.Equals(o));

                                        var rolePermission = new RolePermission
                                        {
                                            RoleId = x.Id,
                                            PermissionId = permission?.Id ?? Guid.Empty,
                                            PermissionCode = o
                                        };
                                        return rolePermission;
                                    }));
                            }
                        });
                    try
                    {
                        await applicationDbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
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
            var uri = string.Empty;
            if (clientSection != null)
            {
                uri = clientSection.GetChildren().FirstOrDefault()?.Value;
            }

            return uri;
        }
    }
}
using GR.Core.Helpers;
using GR.Identity.Data;
using GR.Identity.IdentityServer4.Extensions;
using GR.Identity.Seeders;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Identity.Permissions.Abstractions.Helpers;
using GR.Identity.Permissions.Abstractions.Permissions;

namespace GR.Identity.IdentityServer4.Seeders
{
    public static class IdentityServerConfigDbSeeder
    {
        public static async Task SeedAsync<TIdentityServer4Configurator>(TIdentityServer4Configurator configurator, ConfigurationDbContext context, ApplicationDbContext applicationDbContext,
            IConfiguration configuration, IHostingEnvironment env) where TIdentityServer4Configurator : IdentityServer4Configurator
        {
            var clientUrls = new Dictionary<string, string>
            {
                ["CORE"] = GetClientUrl(env, configuration, "CORE")
            };

            //Seed clients
            if (!context.Clients.Any())
            {
                try
                {
                    var clients = configurator.GetClients(clientUrls).GetSeedClients(context);
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
                var resources = configurator.GetResources().GetSeedResources(context);
                await context.IdentityResources.AddRangeAsync(resources);

                await context.SaveChangesAsync();
            }

            //Seed api resources
            if (!context.ApiResources.Any())
            {
                var apiResources = configurator.GetApis().GetSeedApiResources(context);
                context.ApiResources.AddRange(apiResources);
                await context.SaveChangesAsync();
            }

            //Seed permissions
            if (!applicationDbContext.Permissions.Any())
            {
                var clients = context.Clients.ToList();
                //Start seed permissions
                await PermissionsInvoker.InvokeAsync();

                var baseDirectory = AppContext.BaseDirectory;
                var entity = JsonParser.ReadObjectDataFromJsonFile<ApplicationDbContextSeed.SeedApplication>(Path.Combine(baseDirectory, "Configuration/IdentityConfiguration.json"));

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
                                        PermissionId = o.Id
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
                                            PermissionId = permission?.Id ?? Guid.Empty
                                        };
                                        return rolePermission;
                                    }));
                            }
                        });
                    await applicationDbContext.PushAsync();
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
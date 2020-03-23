using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Identity.Abstractions.Helpers;
using GR.Identity.Abstractions.ViewModels.SeedViewModels;
using GR.Identity.Clients.Abstractions.Extensions;
using GR.Identity.Permissions.Abstractions;
using GR.Identity.Permissions.Abstractions.Helpers;
using GR.Identity.Permissions.Abstractions.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Identity.Clients.Abstractions.Helpers
{
    public static class ClientsSeeder
    {
        /// <summary>
        /// Seed clients
        /// </summary>
        /// <typeparam name="TConfigurator"></typeparam>
        /// <param name="services"></param>
        /// <param name="configurator"></param>ram>
        /// <returns></returns>
        public static async Task SeedAsync<TConfigurator>(IServiceProvider services, TConfigurator configurator) where TConfigurator : DefaultClientsConfigurator
        {
            var context = services.GetRequiredService<IClientsContext>();
            var configuration = services.GetRequiredService<IConfiguration>();
            var permissionsContext = services.GetRequiredService<IPermissionsContext>();
            var clientsSection = configuration.GetSection(ClientResources.WebClientsSection);
            var sectionClients = clientsSection.Get<Dictionary<string, object>>();
            var clientUrls = sectionClients
                .ToDictionary(sectionClient => sectionClient.Key,
                    sectionClient => GetClientUrl(configuration, sectionClient.Key));

            //Seed clients
            if (!context.Clients.Any())
            {
                var clients = configurator.GetClients(clientUrls).GetSeedClients();
                await context.Clients.AddRangeAsync(clients);

                try
                {
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
                var resources = configurator.GetResources().GetSeedResources();
                await context.IdentityResources.AddRangeAsync(resources);

                await context.PushAsync();
            }

            //Seed api resources
            if (!context.ApiResources.Any())
            {
                var apiResources = configurator.GetApiResources().GetSeedApiResources(context);
                context.ApiResources.AddRange(apiResources);
                await context.PushAsync();
            }

            //Seed permissions
            if (!permissionsContext.Permissions.Any())
            {
                var clients = context.Clients.ToList();
                //Start seed permissions
                await PermissionsProvider.InvokeAsync();

                var baseDirectory = AppContext.BaseDirectory;
                var entity = JsonParser.ReadObjectDataFromJsonFile<IdentitySeedViewModel>(Path.Combine(baseDirectory, IdentityResources.Configuration.DEFAULT_FILE_PATH));

                //Set core permissions for roles
                var coreClientId = clients.FirstOrDefault(y => y.ClientId.Equals("core"))?.Id;
                var corePermissions = permissionsContext.Permissions.Where(x => x.ClientId.Equals(coreClientId));
                if (corePermissions.Any())
                {
                    await permissionsContext.Roles.Where(x => x.ClientId.Equals(coreClientId))
                        .ForEachAsync(async x =>
                        {
                            var configuredPermissions =
                                entity.ApplicationRoles.FirstOrDefault(y => y.Name.Equals(x.Name));
                            if (configuredPermissions == null) return;

                            if (configuredPermissions.Permissions == null || !configuredPermissions.Permissions.Any())
                            {
                                await permissionsContext.RolePermissions.AddRangeAsync(corePermissions.Select(o =>
                                    new RolePermission
                                    {
                                        RoleId = x.Id,
                                        PermissionId = o.Id
                                    }));
                            }
                            else
                            {
                                await permissionsContext.RolePermissions.AddRangeAsync(configuredPermissions.Permissions.Select(
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
                    await permissionsContext.PushAsync();
                }
            }
        }

        /// <summary>
        /// Get uri of client from app settings;
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static string GetClientUrl(IConfiguration configuration, string sectionName)
        {
            var configs = configuration.GetSection(ClientResources.WebClientsSection);
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
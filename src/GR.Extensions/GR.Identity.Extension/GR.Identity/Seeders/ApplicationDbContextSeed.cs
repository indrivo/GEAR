using GR.Core;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Data;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Identity.Abstractions.Helpers;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.Abstractions.ViewModels.SeedViewModels;

namespace GR.Identity.Seeders
{
    public class ApplicationDbContextSeed
    {
        /// <summary>
        /// Seed async all components
        /// </summary>
        /// <param name="context"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public async Task SeedAsync(GearIdentityDbContext context, IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<GearUser>>();
            var roleManager = services.GetRequiredService<RoleManager<GearRole>>();
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ApplicationDbContextSeed));
            var baseDirectory = AppContext.BaseDirectory;
            var data = JsonParser.ReadObjectDataFromJsonFile<IdentitySeedViewModel>(Path.Combine(baseDirectory,
                IdentityResources.Configuration.DEFAULT_FILE_PATH));

            var tenant = new Tenant
            {
                Id = GearSettings.TenantId,
                Name = "Default tenant",
                MachineName = GearSettings.DEFAULT_ENTITY_SCHEMA,
                Created = DateTime.Now,
                Changed = DateTime.Now,
                Author = GlobalResources.Roles.ANONYMOUS_USER
            };

            await context.Tenants.AddAsync(tenant);
            await context.PushAsync();

            if (data == null) return;

            // Check and seed system roles
            if (data.ApplicationRoles.Any())
            {
                foreach (var role in data.ApplicationRoles)
                {
                    var exists = await roleManager.RoleExistsAsync(role.Name);
                    if (exists) continue;
                    role.Created = DateTime.Now;
                    role.Changed = DateTime.Now;
                    role.TenantId = GearSettings.TenantId;
                    await roleManager.CreateAsync(role.Adapt<GearRole>());
                    await context.SaveChangesAsync();
                }
                logger.LogInformation("Roles are synchronized with database");
            }

            // Check and seed users
            if (data.ApplicationUsers.Any())
            {
                foreach (var seedUser in data.ApplicationUsers)
                {
                    var user = seedUser.Adapt<GearUser>();
                    var exists = await userManager.FindByNameAsync(user.UserName);
                    if (exists != null) continue;
                    user.AuthenticationType = IdentityResources.LocalAuthenticationType;
                    user.TenantId = GearSettings.TenantId;
                    var result = await userManager.CreateAsync(user, seedUser.Password);
                    if (!result.Succeeded) continue;
                    if (data.ApplicationRoles.Any())
                    {
                        await userManager.AddToRolesAsync(user, data.ApplicationRoles.Select(x => x.Name));
                    }

                    await context.PushAsync();
                }
                logger.LogInformation("System users are synchronized with database");
            }
        }
    }
}
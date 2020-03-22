using GR.Core;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Enums;
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
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.Abstractions.ViewModels.SeedViewModels;

namespace GR.Identity.Seeders
{
    public class ApplicationDbContextSeed
    {
        /// <summary>
        /// Configuration path
        /// </summary>
        protected const string ConfigurationPath = "Configuration/IdentityConfiguration.json";

        /// <summary>
        /// Seed async all components
        /// </summary>
        /// <param name="context"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public async Task SeedAsync(IdentityDbContext context, IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<GearUser>>();
            var roleManager = services.GetRequiredService<RoleManager<GearRole>>();
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ApplicationDbContextSeed));
            var baseDirectory = AppContext.BaseDirectory;
            var entity = JsonParser.ReadObjectDataFromJsonFile<IdentitySeedViewModel>(Path.Combine(baseDirectory, ConfigurationPath));

            var tenant = new Tenant
            {
                Id = GearSettings.TenantId,
                Name = "Default tenant",
                MachineName = GearSettings.DEFAULT_ENTITY_SCHEMA,
                Created = DateTime.Now,
                Changed = DateTime.Now,
                Author = GlobalResources.Roles.ANONIMOUS_USER
            };

            await context.Tenants.AddAsync(tenant);
            await context.PushAsync();

            if (entity == null) return;

            // Check and seed system roles
            if (entity.ApplicationRoles.Any())
            {
                foreach (var role in entity.ApplicationRoles)
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
            if (entity.ApplicationUsers.Any())
            {
                foreach (var seedUser in entity.ApplicationUsers)
                {
                    var user = seedUser.Adapt<GearUser>();
                    var exists = await userManager.FindByNameAsync(user.UserName);
                    if (exists != null) continue;
                    user.AuthenticationType = AuthenticationType.Local;
                    user.TenantId = GearSettings.TenantId;
                    var result = await userManager.CreateAsync(user, seedUser.Password);
                    if (!result.Succeeded) continue;
                    if (entity.ApplicationRoles.Any())
                    {
                        await userManager.AddToRolesAsync(user, entity.ApplicationRoles.Select(x => x.Name));
                    }

                    await context.PushAsync();
                }
                logger.LogInformation("System users are synchronized with database");
            }
        }
    }
}
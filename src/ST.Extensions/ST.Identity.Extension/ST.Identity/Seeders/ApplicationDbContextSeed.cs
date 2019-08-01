using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ST.Core;
using ST.Core.Helpers;
using ST.Identity.Abstractions;
using ST.Identity.Abstractions.Enums;
using ST.Identity.Data;
using ST.Identity.Data.UserProfiles;
using ST.Identity.Models.RolesViewModels;
using ST.Identity.Models.UserViewModels;

namespace ST.Identity.Seeders
{
    public class ApplicationDbContextSeed
    {
        /// <summary>
        /// Seed async all components
        /// </summary>
        /// <param name="context"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public async Task SeedAsync(ApplicationDbContext context, IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ApplicationDbContextSeed));
            var baseDirectory = AppContext.BaseDirectory;
            var entity = JsonParser.ReadObjectDataFromJsonFile<SeedApplication>(Path.Combine(baseDirectory, "IdentityConfiguration.json"));

            if (entity == null)
            {
                return;
            }
            // Check and seed system roles
            if (entity.ApplicationRoles.Any())
            {
                foreach (var role in entity.ApplicationRoles)
                {
                    var exists = await roleManager.RoleExistsAsync(role.Name);
                    if (exists) continue;
                    role.Created = DateTime.Now;
                    role.Changed = DateTime.Now;
                    role.TenantId = Settings.TenantId;
                    await roleManager.CreateAsync(role.Adapt<ApplicationRole>());
                    await context.SaveChangesAsync();
                }
                logger.LogInformation("Roles are synchronized with database");
            }

            // Check and seed groups
            if (entity.Profiles.Any())
            {
                foreach (var item in entity.Groups)
                {
                    if (context.AuthGroups.Any(x => x.Name.Equals(item.Name))) continue;
                    item.Created = DateTime.Now;
                    item.Changed = DateTime.Now;
                    item.TenantId = Settings.TenantId;
                    context.AuthGroups.Add(item);
                    context.SaveChanges();
                }
                logger.LogInformation("Groups are synchronized with database");
            }

            // Check and seed users
            if (entity.ApplicationUsers.Any())
            {
                var group = context.AuthGroups.FirstOrDefault();
                foreach (var user in entity.ApplicationUsers.Select(x => x.Adapt<ApplicationUser>()).ToList())
                {
                    var exists = await userManager.FindByNameAsync(user.UserName);
                    if (exists != null) continue;
                    var hasher = new PasswordHasher<ApplicationUser>();
                    var passwordHash = hasher.HashPassword(user, user.Password);
                    user.PasswordHash = passwordHash;
                    user.Created = DateTime.Now;
                    user.Changed = DateTime.Now;
                    user.AuthenticationType = AuthenticationType.Local;
                    user.TenantId = Settings.TenantId;
                    var result = await userManager.CreateAsync(user);
                    if (!result.Succeeded) continue;
                    if (entity.ApplicationRoles.Any())
                    {
                        await userManager.AddToRolesAsync(user, entity.ApplicationRoles.Select(x => x.Name));
                    }

                    if (group != null)
                    {
                        var userGroup = new UserGroup
                        {
                            UserId = user.Id,
                            AuthGroupId = group.Id,
                            TenantId = Settings.TenantId
                        };
                        context.UserGroups.Add(userGroup);
                    }
                    context.SaveChanges();
                }
                logger.LogInformation("System users are synchronized with database");
            }
            // Check and seed entities types
            if (entity.Profiles.Any())
            {
                foreach (var item in entity.Profiles)
                {
                    if (context.Profiles.Any(x => x.ProfileName == item.ProfileName)) continue;
                    item.TenantId = Settings.TenantId;
                    context.Profiles.Add(item);
                    context.SaveChanges();
                }
            }
        }

        public class SeedApplication
        {
            /// <summary>
            /// List of system roles
            /// </summary>
            public List<RolesViewModel> ApplicationRoles { get; set; }
            /// <summary>
            /// List of system users
            /// </summary>
            public List<UserSeedViewModel> ApplicationUsers { get; set; }
            /// <summary>
            /// List of profiles
            /// </summary>
            public List<Profile> Profiles { get; set; }
            /// <summary>s
            /// List of groups
            /// </summary>
            public List<AuthGroup> Groups { get; set; }
        }
    }
}
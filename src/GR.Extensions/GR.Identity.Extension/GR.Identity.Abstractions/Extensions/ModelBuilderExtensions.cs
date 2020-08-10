using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Abstractions.Extensions
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Apply identity tables config
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ModelBuilder ApplyIdentityDbContextConfiguration(this ModelBuilder builder)
        {
            const string schema = "Identity";
            builder.Entity<GearRole>().ToTable("Roles", schema);
            builder.Entity<GearUser>().ToTable("Users", schema);
            builder.Entity<IdentityUserRole<Guid>>(x =>
            {
                x.ToTable("UserRoles", schema);
                x.HasKey(k => new { k.UserId, k.RoleId });
            });

            builder.Entity<IdentityUserClaim<Guid>>(x =>
            {
                x.ToTable("UserClaims", schema);
            });

            builder.Entity<IdentityUserLogin<Guid>>(x =>
            {
                x.ToTable("UserLogins", schema);
                x.HasKey(k => new { k.LoginProvider, k.ProviderKey });
            });

            builder.Entity<IdentityRoleClaim<Guid>>(x =>
            {
                x.ToTable("RoleClaims", schema);
            });

            builder.Entity<IdentityUserToken<Guid>>(x =>
            {
                x.ToTable("UserTokens", schema);
                x.HasKey(v => new { v.Name, v.UserId, v.LoginProvider });
            });
            return builder;
        }
    }
}

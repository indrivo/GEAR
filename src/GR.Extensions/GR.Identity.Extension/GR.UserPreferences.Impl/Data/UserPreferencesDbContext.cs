using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using GR.Audit.Abstractions.Models;
using GR.Audit.Contexts;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.UserPreferences.Abstractions;
using GR.UserPreferences.Abstractions.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GR.UserPreferences.Impl.Data
{
    [Author(Authors.LUPEI_NICOLAE)]
    public class UserPreferencesDbContext : TrackerDbContext, IUserPreferencesContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Identity";

        /// <summary>
        /// Check if is migration mode
        /// </summary>
        public static bool IsMigrationMode { get; set; } = true;

        public UserPreferencesDbContext(DbContextOptions<UserPreferencesDbContext> options) : base(options)
        {
        }

        #region Entitities

        /// <summary>
        /// User preferences
        /// </summary>
        public virtual DbSet<UserPreference> UserPreferences { get; set; }

        #endregion


        #region Identity

        [NotMapped]
        public DbSet<GearRole> Roles { get; set; }
        [NotMapped]
        public DbSet<IdentityUserRole<Guid>> UserRoles { get; set; }
        [NotMapped]
        public virtual DbSet<IdentityUserLogin<Guid>> UserLogins { get; set; }
        [NotMapped]
        public virtual DbSet<IdentityRoleClaim<Guid>> RoleClaims { get; set; }
        [NotMapped]
        public virtual DbSet<IdentityUserToken<Guid>> UserTokens { get; set; }
        [NotMapped]
        public DbSet<GearUser> Users { get; set; }
        [NotMapped]
        public DbSet<Tenant> Tenants { get; set; }

        #endregion

        /// <summary>
        /// Model configurations
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
            builder.Entity<UserPreference>(o =>
            {
                o.HasKey(x => new { x.Key, x.UserId });
            });


            //Configure Identity table naming
            builder.Entity<GearRole>().ToTable("Roles", Schema);
            builder.Entity<GearUser>().ToTable("Users", Schema);
            builder.Entity<IdentityUserRole<Guid>>(x =>
            {
                x.ToTable("UserRoles", Schema);
                x.HasKey(k => new { k.UserId, k.RoleId });
            });

            builder.Entity<IdentityUserClaim<Guid>>(x =>
            {
                x.ToTable("UserClaims", Schema);
            });

            builder.Entity<IdentityUserLogin<Guid>>(x =>
            {
                x.ToTable("UserLogins", Schema);
                x.HasKey(k => new { k.LoginProvider, k.ProviderKey });
            });

            builder.Entity<IdentityRoleClaim<Guid>>(x =>
            {
                x.ToTable("RoleClaims", Schema);
            });

            builder.Entity<IdentityUserToken<Guid>>(x =>
            {
                x.ToTable("UserTokens", Schema);
                x.HasKey(v => new { v.Name, v.UserId, v.LoginProvider });
            });

            if (IsMigrationMode)
            {
                builder.Ignore<TrackAudit>();
                builder.Ignore<TrackAuditDetails>();

                builder.Ignore<GearUser>();
                builder.Ignore<GearRole>();
                builder.Ignore<Tenant>();
                builder.Ignore<IdentityUserRole<Guid>>();
                builder.Ignore<IdentityUserClaim<Guid>>();
                builder.Ignore<IdentityUserLogin<Guid>>();
                builder.Ignore<IdentityRoleClaim<Guid>>();
                builder.Ignore<IdentityUserToken<Guid>>();
            }
        }

        /// <summary>
        /// Invoke seed data
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services) => Task.CompletedTask;
    }
}

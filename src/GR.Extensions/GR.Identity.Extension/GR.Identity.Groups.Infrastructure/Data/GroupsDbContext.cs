using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using GR.Audit.Contexts;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.Groups.Abstractions;
using GR.Identity.Groups.Abstractions.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Groups.Infrastructure.Data
{
    public class GroupsDbContext : TrackerDbContext, IGroupContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Identity";

        /// <summary>
        /// Check if is migration mode
        /// </summary>
        public static bool IsMigrationMode { get; set; } = false;

        public GroupsDbContext(DbContextOptions<GroupsDbContext> options) : base(options)
        {

        }

        /// <summary>
        /// All groups
        /// </summary>
        public virtual DbSet<Group> Groups { get; set; }

        /// <summary>
        /// User groups
        /// </summary>
        public virtual DbSet<UserGroup> UserGroups { get; set; }

        /// <summary>
        /// Group permissions
        /// </summary>
        public virtual DbSet<GroupPermission> GroupPermissions { get; set; }

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
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);

            builder.Entity<UserGroup>()
                .HasKey(ug => new { ug.GroupId, ug.UserId });

            builder.Entity<UserGroup>()
                .HasIndex(x => x.TenantId);

            builder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(ug => ug.UserGroups)
                .HasForeignKey(ug => ug.GroupId);

            builder.Entity<GearUser>()
                .HasMany<UserGroup>()
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);


            //Configure Identity table naming
            builder.Entity<GearRole>().ToTable("Roles");
            builder.Entity<GearUser>().ToTable("Users");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");


            if (IsMigrationMode)
            {
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
        /// On seed data
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }
    }
}

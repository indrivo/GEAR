using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using GR.Audit.Contexts;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.AddressModels;
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
        public DbSet<GearUser> Users { get; set; }
        [NotMapped]
        public DbSet<Tenant> Tenants { get; set; }
        [NotMapped]
        public DbSet<Country> Countries { get; set; }
        [NotMapped]
        public DbSet<StateOrProvince> StateOrProvinces { get; set; }
        [NotMapped]
        public DbSet<Address> Addresses { get; set; }
        [NotMapped]
        public DbSet<District> Districts { get; set; }

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


            if (IsMigrationMode)
            {
                builder.Ignore<GearUser>();
                builder.Ignore<GearRole>();
                builder.Ignore<IdentityUserRole<Guid>>();
                builder.Ignore<Tenant>();
                builder.Ignore<Country>();
                builder.Ignore<StateOrProvince>();
                builder.Ignore<Address>();
                builder.Ignore<District>();
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

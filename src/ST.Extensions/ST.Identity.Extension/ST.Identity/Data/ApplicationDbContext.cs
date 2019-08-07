using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ST.Audit.Contexts;
using ST.Core.Abstractions;
using ST.Core.Helpers.DbContexts;
using ST.Identity.Abstractions;
using ST.Identity.Abstractions.Models;
using ST.Identity.Abstractions.Models.MultiTenants;
using ST.Identity.Abstractions.Models.Permmisions;
using ST.Identity.Abstractions.Models.UserProfiles;
using ST.Identity.Extensions;

namespace ST.Identity.Data
{
    public class ApplicationDbContext : TrackerIdentityDbContext<ApplicationUser, ApplicationRole, string>, IIdentityContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Identity";
        /// <summary>
        /// Options
        /// </summary>
        private DbContextOptions<ApplicationDbContext> Options { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Options = options;
        }

        #region Permissions Store
        public virtual DbSet<Tenant> Tenants { get; set; }
        public virtual DbSet<AuthGroup> AuthGroups { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<GroupPermission> GroupPermissions { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<RoleProfile> RoleProfiles { get; set; }

        #endregion Permissions Store

        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
            builder.Entity<ApplicationRole>().ToTable("Roles");
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<Permission>().ToTable("Permissions");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");


            builder.Entity<RoleProfile>().HasKey(ug => new { ug.ApplicationRoleId, ug.ProfileId });

            builder.Entity<UserGroup>()
                .HasKey(ug => new { ug.AuthGroupId, ug.UserId });

            builder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(ug => ug.UserGroups)
                .HasForeignKey(ug => ug.UserId);

            builder.Entity<UserGroup>()
                .HasOne(ug => ug.AuthGroup)
                .WithMany(ug => ug.UserGroups)
                .HasForeignKey(ug => ug.AuthGroupId);

            builder.Entity<ApplicationUser>(x => { x.Property(p => p.Id).HasConversion<Guid>(); });
            builder.Entity<ApplicationRole>(x => { x.Property(p => p.Id).HasConversion<Guid>(); });

            builder.RegisterIndexes();
        }

        public virtual DbSet<T> SetEntity<T>() where T : class, IBaseModel
        {
            return this.Set<T>();
        }
    }
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<ApplicationDbContext, ApplicationDbContext>.CreateFactoryDbContext();
        }
    }
}
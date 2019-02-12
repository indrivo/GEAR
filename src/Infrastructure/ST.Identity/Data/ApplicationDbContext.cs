using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;
using ST.Audit.Contexts;
using ST.Entities.Services;

namespace ST.Identity.Data
{
    public class ApplicationDbContext : TrackerIdentityDbContext<ApplicationUser, ApplicationRole, string>
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
        // ReSharper disable once SuggestBaseTypeForParameter
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Options = options;
        }

        #region Permissions Store

        public DbSet<AuthGroup> AuthGroups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<GroupPermission> GroupPermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<RoleProfile> RoleProfiles { get; set; }

        #endregion Permissions Store

        /// <summary>
        /// Dynamic Entities
        /// </summary>
        public readonly Dictionary<string, Entity> Entities =
            EntityStorage.DynamicEntities.ToDictionary(x => x.Name, x => x);

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
        }
    }
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BPM.DEV;Trusted_Connection=True;");
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
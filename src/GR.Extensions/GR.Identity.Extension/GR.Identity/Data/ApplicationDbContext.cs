using GR.Audit.Contexts;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models;
using GR.Identity.Abstractions.Models.AddressModels;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.Extensions;
using GR.Identity.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using GR.Identity.Permissions.Abstractions;
using GR.Identity.Permissions.Abstractions.Permissions;

namespace GR.Identity.Data
{
    public class ApplicationDbContext : TrackerIdentityDbContext<GearUser, GearRole, Guid>, IIdentityContext, IPermissionsContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Identity";

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #region Permissions Store

        public virtual DbSet<Tenant> Tenants { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }

        #endregion

        #region Address

        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<StateOrProvince> StateOrProvinces { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<District> Districts { get; set; }

        #endregion Address

        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
            builder.Entity<GearRole>().ToTable("Roles");
            builder.Entity<GearUser>().ToTable("Users");
            builder.Entity<Permission>().ToTable("Permissions");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

            builder.Entity<Country>().HasKey(k => k.Id);
            builder.Entity<StateOrProvince>().HasKey(k => k.Id);
            builder.Entity<StateOrProvince>()
                .Property(k => k.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<StateOrProvince>().Property(x => x.Id);
            builder.Entity<GearUser>()
                .HasMany(x => x.Addresses)
                .WithOne(x => x.ApplicationUser)
                .OnDelete(DeleteBehavior.Cascade);

            //seed countries
            var countries = ApplicationDbContextSeeder.GetCountriesFromJsonFile();
            foreach (var country in countries)
            {
                var cities = country.StatesOrProvinces;
                country.StatesOrProvinces = null;
                builder.Entity<Country>().HasData(country);
                builder.Entity<StateOrProvince>().HasData(cities);
            }

            builder.RegisterIndexes();
        }

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            var seeder = new ApplicationDbContextSeed();
            seeder.SeedAsync(this, services).Wait();
            return Task.CompletedTask;
        }
    }
}
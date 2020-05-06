using GR.Audit.Contexts;
using GR.Identity.Profile.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using GR.Audit.Abstractions.Models;
using GR.Identity.Abstractions;
using GR.Identity.Profile.Abstractions.Models.AddressModels;
using GR.Localization.Abstractions.Models.Countries;
using ProfileModels = GR.Identity.Profile.Abstractions.Models;

namespace GR.Identity.Profile.Data
{
    public class ProfileDbContext : TrackerDbContext, IProfileContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Identity";

        /// <summary>
        /// Country Schema
        /// </summary>
        private const string CountrySchema = "Localization";

        /// <summary>
        /// Check if is migration mode
        /// </summary>
        public static bool IsMigrationMode { get; set; } = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options)
        {
        }

        public virtual DbSet<ProfileModels.Profile> Profiles { get; set; }
        public virtual DbSet<ProfileModels.UserProfile> UserProfiles { get; set; }
        public virtual DbSet<ProfileModels.RoleProfile> RoleProfiles { get; set; }
        public virtual DbSet<Address> UserAddresses { get; set; }

        #region Ignore

        [NotMapped]
        public virtual DbSet<Country> Countries { get; set; }
        [NotMapped]
        public virtual DbSet<StateOrProvince> StateOrProvinces { get; set; }

        #endregion


        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);

            builder.Entity<ProfileModels.RoleProfile>().HasKey(ug => new { ug.RoleId, ug.ProfileId });
            builder.Entity<ProfileModels.UserProfile>().HasKey(ug => new { ug.UserId, ug.RoleProfileId });
            builder.Entity<ProfileModels.UserProfile>().HasIndex(x => x.UserId);

            builder.Entity<ProfileModels.Profile>()
                .HasIndex(x => x.TenantId);

            builder.Entity<Country>().ToTable("Countries", CountrySchema);
            builder.Entity<StateOrProvince>().ToTable("StateOrProvinces", CountrySchema);
            builder.Entity<Address>().HasIndex(x => x.CountryId);
            builder.Entity<Address>().HasIndex(x => x.StateOrProvinceId);
            builder.Entity<Address>().HasIndex(x => x.UserId);

            builder.Entity<GearUser>().ToTable("Users", Schema);

            if (IsMigrationMode)
            {
                builder.Ignore<Country>();
                builder.Ignore<StateOrProvince>();

                builder.Ignore<GearUser>();

                builder.Ignore<TrackAudit>();
                builder.Ignore<TrackAuditDetails>();
            }
        }

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }
    }
}
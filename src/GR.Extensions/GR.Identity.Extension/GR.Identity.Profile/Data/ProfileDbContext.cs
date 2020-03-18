using GR.Audit.Contexts;
using GR.Identity.Profile.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
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

        public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options)
        {
        }

        public virtual DbSet<ProfileModels.Profile> Profiles { get; set; }
        public virtual DbSet<ProfileModels.UserProfile> UserProfiles { get; set; }
        public virtual DbSet<ProfileModels.RoleProfile> RoleProfiles { get; set; }


        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);

            builder.Entity<ProfileModels.RoleProfile>().HasKey(ug => new { ug.RoleId, ug.ProfileId });

            builder.Entity<ProfileModels.Profile>()
                .HasIndex(x => x.TenantId);
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
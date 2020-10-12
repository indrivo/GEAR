using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using GR.Audit.Abstractions.Models;
using GR.Audit.Contexts;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.IdentityDocuments.Abstractions;
using GR.IdentityDocuments.Abstractions.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GR.IdentityDocuments.Data
{
    public class IdentityDocumentsDbContext : TrackerDbContext, IIdentityDocumentsContext
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

        public IdentityDocumentsDbContext(DbContextOptions<IdentityDocumentsDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Identity documents
        /// </summary>
        public virtual DbSet<IdentityDocument> IdentityDocuments { get; set; }

        /// <summary>
        /// User kyc
        /// </summary>
        public virtual DbSet<UserKyc> UserKyc { get; set; }


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

            //Configure Identity table naming
            builder.ApplyIdentityDbContextConfiguration();

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
        /// Invoke seed
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }
    }
}

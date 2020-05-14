using System;
using System.Threading.Tasks;
using GR.Audit.Abstractions.Models;
using GR.Audit.Contexts;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.Localization.Data
{
    public class TranslationsDbContext : TrackerDbContext, ILocalizationContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Localization";

        /// <summary>
        /// Check if is migration mode
        /// </summary>
        public static bool IsMigrationMode { get; set; } = true;

        public TranslationsDbContext(DbContextOptions<TranslationsDbContext> options) : base(options)
        {
        }

        #region Entitities

        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Translation> Translations { get; set; }
        public virtual DbSet<TranslationItem> TranslationItems { get; set; }

        #endregion


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);

            builder.Entity<Language>(o =>
            {
                o.HasKey(x => new { x.Id });
                o.HasIndex(x => x.Identifier);
            });

            builder.Entity<Translation>(o =>
            {
                o.HasKey(x => new { x.Id });
                o.HasIndex(x => x.Key);
            });

            if (IsMigrationMode)
            {
                builder.Ignore<TrackAudit>();
                builder.Ignore<TrackAuditDetails>();
            }
        }

        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }
    }
}

using System;
using System.Threading.Tasks;
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
        }

        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }
    }
}

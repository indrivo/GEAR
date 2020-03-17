using GR.Core.Abstractions;
using GR.Localization.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.Localization.Abstractions
{
    public interface ILocalizationContext : IDbContext
    {
        /// <summary>
        /// Languages
        /// </summary>
        DbSet<Language> Languages { get; set; }

        /// <summary>
        /// Translations
        /// </summary>
        DbSet<Translation> Translations { get; set; }

        /// <summary>
        /// Items
        /// </summary>
        DbSet<TranslationItem> TranslationItems { get; set; }
    }
}
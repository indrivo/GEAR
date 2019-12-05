using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace GR.Localization.Abstractions.Extensions
{
	public static class LocalizationExtensions
    {
        public static LocalizedString GetForLanguage(this IStringLocalizer localizer, string key, string languageIdentifier)
        {
            return localizer.WithCulture(new CultureInfo(languageIdentifier)).GetString(key);
        }

        public static IEnumerable<LocalizedString> GetAllForLanguage(this IStringLocalizer localizer, string languageIdentifier)
        {
            return localizer.WithCulture(new CultureInfo(languageIdentifier)).GetAllStrings();
        }
    }
}

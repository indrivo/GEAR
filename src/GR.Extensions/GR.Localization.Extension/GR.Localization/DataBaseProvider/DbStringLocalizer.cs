using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GR.Cache.Abstractions;
using GR.Core.Extensions;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Models.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace GR.Localization.DataBaseProvider
{
    public class DbStringLocalizer : IStringLocalizer
    {
        #region Private fields

        private string _language;

        #endregion

        #region Constants
        public const string SessionStoreKeyNameDefault = "lang";
        public const string DefaultLanguage = "en";
        #endregion

        #region Injectable

        /// <summary>
        /// Inject localization context
        /// </summary>
        private readonly ILocalizationContext _localizationContext;

        /// <summary>
        /// Inject cache
        /// </summary>
        private readonly ICacheService _cache;

        #endregion

        public DbStringLocalizer(ILocalizationContext localizationContext, ICacheService cache, IHttpContextAccessor httpAccessor, IOptionsSnapshot<LocalizationConfig> locConfig)
        {
            _localizationContext = localizationContext;
            _cache = cache;

            var sessionKey = locConfig.Value.SessionStoreKeyName ?? SessionStoreKeyNameDefault;
            var defaultLanguage = locConfig.Value.DefaultLanguage ?? DefaultLanguage;

            var val = httpAccessor.HttpContext.Session.GetString(sessionKey);

            if (string.IsNullOrEmpty(val))
            {
                httpAccessor.HttpContext.Session.SetString(sessionKey, defaultLanguage);
            }
            _language = httpAccessor.HttpContext.Session.GetString(SessionStoreKeyNameDefault);
        }

        /// <summary>
        /// Get all strings
        /// </summary>
        /// <param name="includeParentCultures"></param>
        /// <returns></returns>
        public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        => _localizationContext.TranslationItems
            .Include(x => x.Translation)
            .Where(x => x.Identifier.Equals(_language)).Select(x => new LocalizedString(x.Translation.Key, x.Value, false))
            .ToList();

        public virtual IStringLocalizer WithCulture(CultureInfo culture)
        {
            _language = culture.TwoLetterISOLanguageName;
            return this;
        }

        public virtual LocalizedString this[string name]
        {
            get
            {
                var key = $"_db_key_{_language}_{name}";
                var translation = _cache.GetAsync<string>(key).ExecuteAsync();
                if (translation != null) return new LocalizedString(name, translation);
                var dbTranslation = _localizationContext.TranslationItems
                    .Include(x => x.Translation)
                    .FirstOrDefault(x => x.Identifier.Equals(_language) && x.Translation.Key.Equals(name));
                if (dbTranslation == null) return new LocalizedString(name, $"[{name}]", true);
                _cache.SetAsync(key, dbTranslation.Value);
                return new LocalizedString(name, dbTranslation.Value);
            }
        }

        /// <summary>
        /// Get translated string
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public virtual LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var translated = this[name];
                var value = string.Format(translated, arguments);
                return new LocalizedString(name, translated.ResourceNotFound
                    ? translated
                    : value, translated.ResourceNotFound);
            }
        }
    }
}

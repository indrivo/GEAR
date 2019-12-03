using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GR.Cache.Abstractions;
using GR.Core.Extensions;
using GR.Localization.Abstractions.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GR.Localization
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        #region DependencyInjection Fields
        private readonly IHostingEnvironment _env;
        private readonly IOptionsSnapshot<LocalizationConfig> _locConfig;
        private readonly ICacheService _cache;
        #endregion

        #region Private Fields
        private string _language;
        private readonly string _path;
        #endregion

        #region Constants
        public const string SessionStoreKeyNameDefault = "lang";
        public const string PathDefault = "Localization/";
        public const string DefaultLanguage = "en";
        #endregion

        #region Constructors
        public JsonStringLocalizer(IHostingEnvironment env,
            IHttpContextAccessor httpAccessor,
            IOptionsSnapshot<LocalizationConfig> locConfig,
            ICacheService cache)
        {
            _env = env;
            _locConfig = locConfig;
            _cache = cache;
            var sessionKey = _locConfig.Value.SessionStoreKeyName ?? SessionStoreKeyNameDefault;
            var defaultLanguage = _locConfig.Value.DefaultLanguage ?? DefaultLanguage;

            var val = httpAccessor.HttpContext.Session.GetString(sessionKey);

            if (string.IsNullOrEmpty(val))
            {
                httpAccessor.HttpContext.Session.SetString(sessionKey, defaultLanguage);
            }

            _language = httpAccessor.HttpContext.Session.GetString(SessionStoreKeyNameDefault);
            _path = _locConfig.Value.Path ?? PathDefault;
        }
        #endregion

        #region IStringLocalizer Implementation
        public LocalizedString this[string name]
        {
            get
            {
                var filePath = GetFilePath();

                var exists = File.Exists(filePath);
                if (!exists)
                {
                    return new LocalizedString(name, $"[{name}]", true);
                }

                var cacheKey = $"{_locConfig.Value.SessionStoreKeyName}_{_language}";
                var locKey = $"{cacheKey}_{name}";

                var cacheTranslated = _cache.GetAsync<string>(locKey).ExecuteAsync();
                if (string.IsNullOrEmpty(cacheTranslated))
                {
                    Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var value = PullDeserialize<string>(name, fileStream);

                    var translated = value ?? $"[{name}]";
                    var resourceNotFound = value == null;
                    if (!resourceNotFound)
                    {
                        _cache.SetAsync(locKey, translated).ExecuteAsync();
                    }
                    return new LocalizedString(name, translated, resourceNotFound);
                }
                else
                {
                    return new LocalizedString(name, cacheTranslated, false);
                }
            }
        }

        /// <summary>
        /// This is used to deserialize only one specific value from
        /// the json without loading the entire object.
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize</typeparam>
        /// <param name="propertyName">Name of the property to get from json</param>
        /// <param name="str"><see cref="Stream"/> from where to read the json</param>
        /// <returns>Deserialized property from the json</returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        private T PullDeserialize<T>(string propertyName, Stream str)
        {
            if (propertyName == null)
                throw new System.ArgumentNullException(nameof(propertyName));

            if (str == null)
                throw new System.ArgumentNullException(nameof(str));

            using (str)
            using (var sReader = new StreamReader(str))
            using (var reader = new JsonTextReader(sReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName
                        && (string)reader.Value == propertyName)
                    {
                        reader.Read();
                        var serializer = new JsonSerializer();
                        return serializer.Deserialize<T>(reader);
                    }
                }
                return default;
            }
        }

        public LocalizedString this[string name, params object[] arguments]
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

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var filePath = GetFilePath();

            var exists = File.Exists(filePath);
            if (!exists)
            {
                Enumerable.Empty<LocalizedString>();
            }

            using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sReader = new StreamReader(stream))
            using (var reader = new JsonTextReader(sReader))
            {
                reader.SupportMultipleContent = true;
                var obj = JObject.Load(reader);
                var properties = obj.Properties();
                foreach (var property in properties)
                {
                    var value = property.Value?.Value<string>();
                    var name = property.Name;
                    yield return new LocalizedString(name, value ?? $"[{name}]", property.Value == null);
                }
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            _language = culture.TwoLetterISOLanguageName;
            return this;
        }
        #endregion

        private string GetFilePath()
        {
            var paths = new string[]
            {
                _env.ContentRootPath,
                string.Format("{0}/{1}{2}", _path, _language, ".json")
            };
            return Path.Combine(paths);
        }
    }
}

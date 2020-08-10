using System.Collections.Generic;
using System.Globalization;
using System.IO;
using GR.Cache.Abstractions;
using GR.Core.Extensions;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Helpers;
using GR.Localization.Abstractions.Models.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GR.Localization.JsonStringProvider
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        #region DependencyInjection Fields
        private readonly IHostingEnvironment _env;
        private readonly ICacheService _cache;
        private readonly ILocalizationService _service;
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
            ICacheService cache, ILocalizationService service)
        {
            _env = env;
            _cache = cache;
            _service = service;
            var sessionKey = locConfig.Value.SessionStoreKeyName ?? SessionStoreKeyNameDefault;
            var defaultLanguage = locConfig.Value.DefaultLanguage ?? DefaultLanguage;

            if (httpAccessor.HttpContext == null)
            {
                _language = DefaultLanguage;
            }
            else
            {
                var val = httpAccessor.HttpContext.Session.GetString(sessionKey);

                if (string.IsNullOrEmpty(val))
                {
                    httpAccessor.HttpContext.Session.SetString(sessionKey, defaultLanguage);
                }

                if (httpAccessor.HttpContext.Request.Headers.ContainsKey(LocalizationResources.XLocalizationIdentifier))
                {
                    _language = httpAccessor.HttpContext.Request.Headers[LocalizationResources.XLocalizationIdentifier];
                }
                else
                {
                    _language = httpAccessor.HttpContext.Session.GetString(SessionStoreKeyNameDefault);
                }
            }

            _path = locConfig.Value.Path ?? PathDefault;
        }
        #endregion

        #region IStringLocalizer Implementation
        public LocalizedString this[string name]
        {
            get
            {
                var filePath = GetFilePath();

                var exists = File.Exists(filePath);
                if (!exists) return new LocalizedString(name, $"[{name}]", true);
                var locKey = _service.GenerateKey(_language, name);//$"{cacheKey}_{name}";

                var cacheTranslated = _cache.GetAsync<string>(locKey).ExecuteAsync();
                if (!string.IsNullOrEmpty(cacheTranslated)) return new LocalizedString(name, cacheTranslated, false);
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
            if (!exists) yield break;

            using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sReader = new StreamReader(stream))
            using (var reader = new JsonTextReader(sReader))
            {
                reader.SupportMultipleContent = true;
                var obj = JObject.Load(reader);
                var properties = obj.Properties();
                foreach (var property in properties)
                {
                    var value = property.Value.Value<string>();
                    var name = property.Name;
                    yield return new LocalizedString(name, value ?? $"[{name}]", value == null);
                }
            }
        }

        public virtual IStringLocalizer WithCulture(CultureInfo culture)
        {
            _language = culture.TwoLetterISOLanguageName;
            return this;
        }
        #endregion

        private string GetFilePath()
        {
            var paths = new[]
            {
                _env.ContentRootPath,
                $"{_path}/{_language}.json"
            };
            return Path.Combine(paths);
        }
    }
}

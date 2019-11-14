using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GR.Localization.Abstractions.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GR.Core.Extensions;

namespace GR.Localization
{
    public class JsonStringLocalizer : IStringLocalizer
	{
        #region DependencyInjection Fields
        private readonly IHostingEnvironment _env;
        private readonly IOptionsSnapshot<LocalizationConfig> _locConfig;
        private readonly IDistributedCache _cache;
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
            IDistributedCache cache)
		{
			_env = env;
            _locConfig = locConfig;
            _cache = cache;
            string sessionKey = _locConfig.Value.SessionStoreKeyName ?? SessionStoreKeyNameDefault;
            string defaultLanguage = _locConfig.Value.DefaultLanguage ?? DefaultLanguage;

            string val = httpAccessor.HttpContext.Session.GetString(sessionKey);

            if(string.IsNullOrEmpty(val))
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
                string filePath = GetFilePath();

                bool exists = File.Exists(filePath);
                if (!exists)
                {
                    return new LocalizedString(name, $"[{name}]", true);
                }

                string cacheKey = $"{_locConfig.Value.SessionStoreKeyName}_{_language}";
                string locKey = $"{cacheKey}_{name}";

                string cacheTranslated = _cache.GetString(locKey);
                if (string.IsNullOrEmpty(cacheTranslated))
                {
                    Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    string value = PullDeserialize<string>(name, fileStream);

                    string translated = value ?? $"[{name}]";
                    bool resourceNotFound = value == null;
                    if (!resourceNotFound)
                    {
                        _cache.SetString(locKey, translated);
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
        /// <returns>Deserialized propert from the json</returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        private T PullDeserialize<T>(string propertyName, Stream str)
        {
            if (propertyName == null)
                throw new System.ArgumentNullException(nameof(propertyName));

            if (str == null)
                throw new System.ArgumentNullException(nameof(str));

            using (str)
            using (StreamReader sReader = new StreamReader(str))
            using (JsonTextReader reader = new JsonTextReader(sReader))
            {
                while(reader.Read())
                {
                    if(reader.TokenType == JsonToken.PropertyName
                        && (string)reader.Value == propertyName)
                    {
                        reader.Read();
                        JsonSerializer serializer = new JsonSerializer();
                        return serializer.Deserialize<T>(reader);
                    }
                }
                return default(T);
            }
        }

		public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                LocalizedString translated = this[name];
                string value = string.Format(translated, arguments);
                return new LocalizedString(name, translated.ResourceNotFound
                    ? translated
                    : value, translated.ResourceNotFound);
            }
        }

		public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
		{
            string filePath = GetFilePath();

            bool exists = File.Exists(filePath);
            if (!exists)
            {
                Enumerable.Empty<LocalizedString>();
            }

			using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (StreamReader sReader = new StreamReader(stream))
			using (JsonTextReader reader = new JsonTextReader(sReader))
			{
				reader.SupportMultipleContent = true;
                JObject obj = JObject.Load(reader);
                IEnumerable<JProperty> properties = obj.Properties();
				foreach (JProperty property in properties)
				{
                    string value = property.Value?.Value<string>();
					string name = property.Name;
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
            string[] paths = new string[]
            {
                _env.ContentRootPath,
                string.Format("{0}/{1}{2}", _path, _language, ".json")
            };
            return Path.Combine(paths);
        }
    }
}

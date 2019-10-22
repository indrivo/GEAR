using System.Collections.Generic;
using GR.Localization.Abstractions.Helpers.Comparators;

namespace GR.Localization.Abstractions.Models
{
	public class LocalizationConfig
    {
        /// <summary>
        /// These backing fields will have default values in case
        /// the json configuration for languages does not exist.
        /// </summary>
        #region Backing fields Default Values
        HashSet<Language> _defaultLanguages = new HashSet<Language>(new LanguageEqualityComparer())
        {
            new Language { Identifier="en", Name = "English"}
        };

        string _path = "Localization";
        string _sessionStoreKeyName = "lang";
        string _defaultLanguage = "en";
        #endregion

        /// <summary>
        /// List of supported languages represented by
        /// a <see cref="Language"/> object. 
        /// </summary>
        public HashSet<Language> Languages
        {
            get
            {
                return _defaultLanguages;
            }
            set
            {
                _defaultLanguages = value;
            }
        }

        /// <summary>
        /// Path where the json localization files are located
        /// </summary>
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        /// <summary>
        /// Name of the key in Session that stores the
        /// current selected language.
        /// </summary>
        public string SessionStoreKeyName
        {
            get
            {
                return _sessionStoreKeyName;
            }
            set
            {
                _sessionStoreKeyName = value;
            }
        }

        /// <summary>
        /// Default language used in localization
        /// </summary>
        public string DefaultLanguage
        {
            get
            {
                return _defaultLanguage;
            }
            set
            {
                _defaultLanguage = value;
            }
        }
    }
}

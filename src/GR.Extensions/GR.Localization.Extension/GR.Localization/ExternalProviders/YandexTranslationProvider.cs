using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Models.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YandexTranslateCSharpSdk;

namespace GR.Localization.ExternalProviders
{
    public class YandexTranslationProvider : IExternalTranslationProvider
    {
        #region Injectable

        /// <summary>
        /// Inject yandex translate sdk
        /// </summary>
        private readonly YandexTranslateSdk _yandexTranslateSdk;

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<YandexTranslationProvider> _logger;

        #endregion

        public YandexTranslationProvider(IOptions<LocalizationProviderSettings> options, ILogger<YandexTranslationProvider> logger)
        {
            _logger = logger;
            _yandexTranslateSdk = new YandexTranslateSdk
            {
                ApiKey = options.Value.ApiKey
            };
        }

        /// <summary>
        /// Translate text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fromIdentifier"></param>
        /// <param name="toIdentifier"></param>
        /// <returns></returns>
        public virtual async Task<string> TranslateTextAsync(string text, string fromIdentifier, string toIdentifier)
        {
            try
            {
                return await _yandexTranslateSdk.TranslateTextAsync(text, $"{fromIdentifier}-{toIdentifier}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Yandex Translate SDK throw an exception");
            }

            return null;
        }

        /// <summary>
        /// Translate text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fromIdentifier"></param>
        /// <param name="toIdentifier"></param>
        /// <returns></returns>
        public virtual string TranslateText(string text, string fromIdentifier, string toIdentifier)
        {
            return TranslateTextAsync(text, fromIdentifier, toIdentifier).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Detect language
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual async Task<string> DetectLanguageAsync(string text)
        {
            return await _yandexTranslateSdk.DetectLanguageAsync(text);
        }

        /// <summary>
        /// Get languages
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<string>> GetLanguagesAsync()
        {
            return (await _yandexTranslateSdk.GetLanguagesAsync()).Select(x => x.Key);
        }
    }
}

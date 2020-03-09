using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Models;
using GR.Localization.Abstractions.Models.Config;
using Microsoft.Extensions.Options;
using YandexTranslateCSharpSdk;

namespace GR.Localization
{
    public class YandexTranslationProvider : IExternalTranslationProvider
    {
        #region Injectable

        /// <summary>
        /// Inject yandex translate sdk
        /// </summary>
        private readonly YandexTranslateSdk _yandexTranslateSdk;

        #endregion

        public YandexTranslationProvider(IOptions<LocalizationProviderSettings> options)
        {
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
            return await _yandexTranslateSdk.TranslateText(text, $"{fromIdentifier}-{toIdentifier}");
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
            return await _yandexTranslateSdk.DetectLanguage(text);
        }

        /// <summary>
        /// Get languages
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<string>> GetLanguagesAsync()
        {
            return await _yandexTranslateSdk.GetLanguages();
        }
    }
}

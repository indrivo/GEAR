﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ST.Localization.Abstractions;
using ST.Localization.Abstractions.Models;
using YandexTranslateCSharpSdk;

namespace ST.Localization.Services
{
    public class YandexTranslationProvider : IExternalTranslationProvider
    {
        private readonly YandexTranslateSdk _yandexTranslateSdk;

        public YandexTranslationProvider(IOptions<LocalizationProviderSettings> options)
        {
            _yandexTranslateSdk = new YandexTranslateSdk
            {
                ApiKey = options.Value.ApiKey
            };
        }

        public virtual async Task<string> TranslateTextAsync(string text, string fromIdentifier, string toIdentifier)
        {
            return await _yandexTranslateSdk.TranslateText(text, $"{fromIdentifier}-{toIdentifier}");
        }

        public virtual string TranslateText(string text, string fromIdentifier, string toIdentifier)
        {
            return TranslateTextAsync(text, fromIdentifier, toIdentifier).GetAwaiter().GetResult();
        }

        public virtual async Task<string> DetectLanguageAsync(string text)
        {
            return await _yandexTranslateSdk.DetectLanguage(text);
        }

        public virtual async Task<IEnumerable<string>> GetLanguagesAsync()
        {
            return await _yandexTranslateSdk.GetLanguages();
        }
    }
}

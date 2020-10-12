using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using GR.Localization.Abstractions;
using Microsoft.Extensions.Logging;

namespace GR.Localization.ExternalProviders
{
    public class GoogleTranslationProvider : IExternalTranslationProvider
    {
        #region Injectable

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<GoogleTranslationProvider> _logger;

        #endregion


        public GoogleTranslationProvider(ILogger<GoogleTranslationProvider> logger)
        {
            _logger = logger;
        }

        public virtual Task<string> TranslateTextAsync(string text, string fromIdentifier, string toIdentifier)
        {
            return Task.FromResult(TranslateText(text, fromIdentifier, toIdentifier));
        }

        public virtual string TranslateText(string text, string fromIdentifier, string toIdentifier)
        {
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromIdentifier}&tl={toIdentifier}&dt=t&q={HttpUtility.UrlEncode(text)}";
            var webClient = new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
            var result = webClient.DownloadString(url);
            try
            {
                result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Google fail to translate word");
                return null;
            }
        }

        public virtual Task<string> DetectLanguageAsync(string text)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<string>> GetLanguagesAsync()
        {
            throw new NotImplementedException();
        }
    }
}

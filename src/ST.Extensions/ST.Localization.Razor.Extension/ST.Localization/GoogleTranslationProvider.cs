using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ST.Localization.Abstractions;

namespace ST.Localization.Services
{
    public class GoogleTranslationProvider : IExternalTranslationProvider
    {
        public virtual Task<string> TranslateTextAsync(string text, string fromIdentifier, string toIdentifier)
        {
            throw new NotImplementedException();
        }

        public virtual string TranslateText(string text, string fromIdentifier, string toIdentifier)
        {
            throw new NotImplementedException();
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

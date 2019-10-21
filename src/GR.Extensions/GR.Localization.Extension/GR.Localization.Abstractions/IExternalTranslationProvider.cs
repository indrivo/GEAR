using System.Collections.Generic;
using System.Threading.Tasks;

namespace GR.Localization.Abstractions
{
    public interface IExternalTranslationProvider
    {
        Task<string> TranslateTextAsync(string text, string fromIdentifier, string toIdentifier);
        string TranslateText(string text, string fromIdentifier, string toIdentifier);
        Task<string> DetectLanguageAsync(string text);
        Task<IEnumerable<string>> GetLanguagesAsync();
    }
}

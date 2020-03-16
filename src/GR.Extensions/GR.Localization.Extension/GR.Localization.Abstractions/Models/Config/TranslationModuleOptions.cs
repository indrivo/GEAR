using Microsoft.Extensions.Configuration;

namespace GR.Localization.Abstractions.Models.Config
{
    public class TranslationModuleOptions
    {
        public LocalizationProvider LocalizationProvider { get; set; } = LocalizationProvider.Yandex;
        public IConfiguration Configuration { get; set; }
    }
}

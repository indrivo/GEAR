using Microsoft.Extensions.Configuration;

namespace ST.Localization.Abstractions.Models
{
    public class TranslationModuleOptions
    {
        public LocalizationProvider LocalizationProvider { get; set; } = LocalizationProvider.Yandex;
        public IConfiguration Configuration { get; set; }
    }
}

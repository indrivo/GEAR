using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Localization.Razor.Helpers
{
    public class LocalizationRazorFileConfiguration : StaticFileConfiguration
    {
        public LocalizationRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

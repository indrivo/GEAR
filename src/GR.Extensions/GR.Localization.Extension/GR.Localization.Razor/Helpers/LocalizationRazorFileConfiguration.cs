using GR.Core.StaticFiles;
using Microsoft.AspNetCore.Hosting;

namespace GR.Localization.Razor.Helpers
{
    public class LocalizationRazorFileConfiguration : StaticFileConfiguration
    {
        public LocalizationRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

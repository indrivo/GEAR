using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Core.UI.Razor.DefaultTheme.Helpers
{
    public class DefaultThemeRazorFileConfiguration : StaticFileConfiguration
    {
        public DefaultThemeRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}
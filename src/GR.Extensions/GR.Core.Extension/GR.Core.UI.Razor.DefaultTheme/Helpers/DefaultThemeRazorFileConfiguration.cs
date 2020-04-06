using GR.Core.StaticFiles;
using Microsoft.AspNetCore.Hosting;

namespace GR.Core.UI.Razor.DefaultTheme.Helpers
{
    public class DefaultThemeRazorFileConfiguration : StaticFileConfiguration
    {
        public DefaultThemeRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

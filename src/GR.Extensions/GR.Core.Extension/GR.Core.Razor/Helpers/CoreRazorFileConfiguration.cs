using GR.Core.StaticFiles;
using Microsoft.AspNetCore.Hosting;

namespace GR.Core.Razor.Helpers
{
    public class CoreRazorFileConfiguration : StaticFileConfiguration
    {
        public CoreRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

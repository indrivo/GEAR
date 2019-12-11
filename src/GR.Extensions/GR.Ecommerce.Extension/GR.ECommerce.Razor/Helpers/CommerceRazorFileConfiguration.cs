using Microsoft.AspNetCore.Hosting;
using GR.Core.StaticFiles;

namespace GR.ECommerce.Razor.Helpers
{
    public class CommerceRazorFileConfiguration : StaticFileConfiguration
    {
        public CommerceRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.ECommerce.Razor.Helpers
{
    public class CommerceRazorFileConfiguration : StaticFileConfiguration
    {
        public CommerceRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

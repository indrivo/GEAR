using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.MultiTenant.Razor.Helpers
{
    public class MultiTenantRazorFileConfiguration : StaticFileConfiguration
    {
        public MultiTenantRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using GR.Core.StaticFiles;

namespace GR.MultiTenant.Razor.Helpers
{
    public class MultiTenantRazorFileConfiguration : StaticFileConfiguration
    {
        public MultiTenantRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

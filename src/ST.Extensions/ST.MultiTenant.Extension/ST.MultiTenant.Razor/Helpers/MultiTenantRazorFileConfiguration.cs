using Microsoft.AspNetCore.Hosting;
using ST.Core.StaticFiles;

namespace ST.MultiTenant.Razor.Helpers
{
    public class MultiTenantRazorFileConfiguration : StaticFileConfiguration
    {
        public MultiTenantRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

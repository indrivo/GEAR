using Microsoft.AspNetCore.Hosting;
using GR.Core.StaticFiles;

namespace GR.Entities.Security.Razor.Helpers
{
    public class EntitySecurityRazorFileConfiguration : StaticFileConfiguration
    {
        public EntitySecurityRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

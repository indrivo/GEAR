using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Entities.Security.Razor.Helpers
{
    public class EntitySecurityRazorFileConfiguration : StaticFileConfiguration
    {
        public EntitySecurityRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

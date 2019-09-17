using Microsoft.AspNetCore.Hosting;
using ST.Core.StaticFiles;

namespace ST.Entities.Security.Razor.Helpers
{
    public class EntitySecurityRazorFileConfiguration : StaticFileConfiguration
    {
        public EntitySecurityRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {

        }
    }
}

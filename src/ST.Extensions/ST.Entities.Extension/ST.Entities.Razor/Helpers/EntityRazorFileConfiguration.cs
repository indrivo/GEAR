using Microsoft.AspNetCore.Hosting;
using ST.Core.StaticFiles;

namespace ST.Entities.Razor.Helpers
{
    public class EntityRazorFileConfiguration : StaticFileConfiguration
    {
        public EntityRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

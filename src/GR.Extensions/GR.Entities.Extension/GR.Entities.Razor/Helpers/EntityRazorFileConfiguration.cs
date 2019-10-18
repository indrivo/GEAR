using Microsoft.AspNetCore.Hosting;
using GR.Core.StaticFiles;

namespace GR.Entities.Razor.Helpers
{
    public class EntityRazorFileConfiguration : StaticFileConfiguration
    {
        public EntityRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

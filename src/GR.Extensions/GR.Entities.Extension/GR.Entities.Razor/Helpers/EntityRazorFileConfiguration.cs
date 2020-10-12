using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Entities.Razor.Helpers
{
    public class EntityRazorFileConfiguration : StaticFileConfiguration
    {
        public EntityRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

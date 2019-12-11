using Microsoft.AspNetCore.Hosting;
using GR.Core.StaticFiles;

namespace GR.Forms.Razor.Helpers
{
    public class FormFileConfiguration : StaticFileConfiguration
    {
        public FormFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

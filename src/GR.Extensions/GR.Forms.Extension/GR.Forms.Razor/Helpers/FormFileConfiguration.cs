using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Forms.Razor.Helpers
{
    public class FormFileConfiguration : StaticFileConfiguration
    {
        public FormFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using ST.Core.StaticFiles;

namespace ST.Forms.Razor.Helpers
{
    public class FormFileConfiguration : StaticFileConfiguration
    {
        public FormFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

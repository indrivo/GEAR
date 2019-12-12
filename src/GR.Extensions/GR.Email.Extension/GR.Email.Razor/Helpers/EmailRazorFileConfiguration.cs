using Microsoft.AspNetCore.Hosting;
using GR.Core.StaticFiles;

namespace GR.Email.Razor.Helpers
{
    public class EmailRazorFileConfiguration : StaticFileConfiguration
    {
        public EmailRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

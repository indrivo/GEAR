using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Email.Razor.Helpers
{
    public class EmailRazorFileConfiguration : StaticFileConfiguration
    {
        public EmailRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using ST.Core.StaticFiles;

namespace ST.Email.Razor.Helpers
{
    public class EmailRazorFileConfiguration : StaticFileConfiguration
    {
        public EmailRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

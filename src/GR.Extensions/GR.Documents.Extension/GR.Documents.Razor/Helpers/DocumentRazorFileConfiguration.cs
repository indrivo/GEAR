using GR.Core.StaticFiles;
using Microsoft.AspNetCore.Hosting;

namespace GR.Documents.Razor.Helpers
{
    public class DocumentRazorFileConfiguration : StaticFileConfiguration
    {
        public DocumentRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

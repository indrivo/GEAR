using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Documents.Razor.Helpers
{
    public class DocumentRazorFileConfiguration : StaticFileConfiguration
    {
        public DocumentRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

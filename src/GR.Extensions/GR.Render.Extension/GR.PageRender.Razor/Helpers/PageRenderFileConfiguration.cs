using Microsoft.AspNetCore.Hosting;
using GR.Core.StaticFiles;

namespace GR.PageRender.Razor.Helpers
{
    public class PageRenderFileConfiguration : StaticFileConfiguration
    {
        public PageRenderFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

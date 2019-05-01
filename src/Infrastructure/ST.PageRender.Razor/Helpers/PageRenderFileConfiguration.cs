using Microsoft.AspNetCore.Hosting;
using ST.Core.StaticFiles;

namespace ST.PageRender.Razor.Helpers
{
    public class PageRenderFileConfiguration : StaticFileConfiguration
    {
        public PageRenderFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

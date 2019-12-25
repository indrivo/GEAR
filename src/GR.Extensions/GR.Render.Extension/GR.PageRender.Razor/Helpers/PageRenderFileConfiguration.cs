using GR.Core.StaticFiles;
using Microsoft.AspNetCore.Hosting;

namespace GR.PageRender.Razor.Helpers
{
    public class PageRenderFileConfiguration : StaticFileConfiguration
    {
        public PageRenderFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}
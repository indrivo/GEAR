using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.PageRender.Razor.Helpers
{
    public class PageRenderFileConfiguration : StaticFileConfiguration
    {
        public PageRenderFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}
using Microsoft.AspNetCore.Hosting;
using GR.Core.StaticFiles;

namespace GR.Dashboard.Razor.Helpers
{
    public class DashBoardFileConfiguration : StaticFileConfiguration
    {
        public DashBoardFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using ST.Core.StaticFiles;

namespace ST.Dashboard.Razor.Helpers
{
    public class DashBoardFileConfiguration : StaticFileConfiguration
    {
        public DashBoardFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

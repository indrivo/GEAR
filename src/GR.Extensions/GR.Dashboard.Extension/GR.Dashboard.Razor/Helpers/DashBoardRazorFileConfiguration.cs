using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Dashboard.Razor.Helpers
{
    public class DashBoardFileConfiguration : StaticFileConfiguration
    {
        public DashBoardFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

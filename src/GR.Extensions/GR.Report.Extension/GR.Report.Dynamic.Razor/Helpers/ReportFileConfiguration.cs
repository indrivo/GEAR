using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Report.Dynamic.Razor.Helpers
{
    public class ReportFileConfiguration : StaticFileConfiguration
    {
        public ReportFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

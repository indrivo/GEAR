using Microsoft.AspNetCore.Hosting;
using GR.Core.StaticFiles;

namespace GR.Report.Dynamic.Razor.Helpers
{
    public class ReportFileConfiguration : StaticFileConfiguration
    {
        public ReportFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

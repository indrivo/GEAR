using Microsoft.AspNetCore.Hosting;
using ST.Core.StaticFiles;

namespace ST.Report.Dynamic.Razor.Helpers
{
    public class ReportFileConfiguration : StaticFileConfiguration
    {
        public ReportFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

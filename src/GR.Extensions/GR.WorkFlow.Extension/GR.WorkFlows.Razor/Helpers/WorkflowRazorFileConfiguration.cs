using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.WorkFlows.Razor.Helpers
{
    public class WorkflowRazorFileConfiguration : StaticFileConfiguration
    {
        public WorkflowRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

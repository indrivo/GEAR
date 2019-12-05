using GR.Core.StaticFiles;
using Microsoft.AspNetCore.Hosting;

namespace GR.WorkFlows.Razor.Helpers
{
    public class WorkflowRazorFileConfiguration : StaticFileConfiguration
    {
        public WorkflowRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using GR.Core.StaticFiles;

namespace GR.TaskManager.Razor.Helpers
{
    public class TaskManagerRazorFileConfiguration : StaticFileConfiguration
    {
        public TaskManagerRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

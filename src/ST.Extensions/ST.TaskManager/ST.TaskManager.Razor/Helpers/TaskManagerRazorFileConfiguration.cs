using Microsoft.AspNetCore.Hosting;
using ST.Core.StaticFiles;

namespace ST.TaskManager.Razor.Helpers
{
    public class TaskManagerRazorFileConfiguration : StaticFileConfiguration
    {
        public TaskManagerRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

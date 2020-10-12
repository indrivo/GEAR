using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.TaskManager.Razor.Helpers
{
    public class TaskManagerRazorFileConfiguration : StaticFileConfiguration
    {
        public TaskManagerRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

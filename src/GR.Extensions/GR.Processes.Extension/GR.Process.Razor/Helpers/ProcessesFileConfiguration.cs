using Microsoft.AspNetCore.Hosting;
using GR.Core.StaticFiles;

namespace GR.Process.Razor.Helpers
{
    public class ProcessesFileConfiguration : StaticFileConfiguration
    {
        public ProcessesFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

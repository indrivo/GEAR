using Microsoft.AspNetCore.Hosting;
using ST.Core.StaticFiles;

namespace ST.Process.Razor.Helpers
{
    public class ProcessesFileConfiguration : StaticFileConfiguration
    {
        public ProcessesFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

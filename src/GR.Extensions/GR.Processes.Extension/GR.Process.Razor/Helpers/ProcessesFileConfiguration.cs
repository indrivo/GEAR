using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Process.Razor.Helpers
{
    public class ProcessesFileConfiguration : StaticFileConfiguration
    {
        public ProcessesFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

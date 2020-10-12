using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Calendar.Razor.Helpers
{
    public class InternalCalendarFileConfiguration : StaticFileConfiguration
    {
        public InternalCalendarFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

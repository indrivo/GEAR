using Microsoft.AspNetCore.Hosting;
using GR.Core.StaticFiles;

namespace GR.Calendar.Razor.Helpers
{
    public class InternalCalendarFileConfiguration : StaticFileConfiguration
    {
        public InternalCalendarFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

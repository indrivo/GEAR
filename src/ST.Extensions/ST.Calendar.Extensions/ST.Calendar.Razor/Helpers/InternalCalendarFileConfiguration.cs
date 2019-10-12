using Microsoft.AspNetCore.Hosting;
using ST.Core.StaticFiles;

namespace ST.Calendar.Razor.Helpers
{
    public class InternalCalendarFileConfiguration : StaticFileConfiguration
    {
        public InternalCalendarFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

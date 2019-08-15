using Microsoft.AspNetCore.Hosting;
using ST.Core.StaticFiles;

namespace ST.Notifications.Razor.Helpers
{
    public class NotificationRazorFileConfiguration : StaticFileConfiguration
    {
        public NotificationRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using GR.Core.StaticFiles;

namespace GR.Notifications.Razor.Helpers
{
    public class NotificationRazorFileConfiguration : StaticFileConfiguration
    {
        public NotificationRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

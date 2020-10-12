using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Notifications.Razor.Helpers
{
    public class NotificationRazorFileConfiguration : StaticFileConfiguration
    {
        public NotificationRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

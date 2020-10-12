using GR.Core.StaticFiles;
using Microsoft.Extensions.Hosting;

namespace GR.Notifications.Subscriptions.Razor.Helpers
{
    public class NotificationSubscriptionsRazorFileConfiguration : StaticFileConfiguration
    {
        public NotificationSubscriptionsRazorFileConfiguration(IHostEnvironment environment) : base(environment)
        {
        }
    }
}

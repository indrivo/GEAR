using GR.Core.StaticFiles;
using Microsoft.AspNetCore.Hosting;

namespace GR.Notifications.Subscriptions.Razor.Helpers
{
    public class NotificationSubscriptionsRazorFileConfiguration : StaticFileConfiguration
    {
        public NotificationSubscriptionsRazorFileConfiguration(IHostingEnvironment environment) : base(environment)
        {
        }
    }
}

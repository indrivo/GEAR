using GR.Core.Events;
using GR.Core.Events.EventArgs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace GR.Core.Razor.Extensions
{
    public static class ApplicationLifeTimeExtensions
    {
        public static IHostApplicationLifetime RegisterAppEvents(this IHostApplicationLifetime lifeTime, IApplicationBuilder app, string appName)
        {
            lifeTime.ApplicationStarted.Register(() =>
                SystemEvents.Application.ApplicationStarted(new ApplicationStartedEventArgs
                {
                    Services = app.ApplicationServices,
                    AppIdentifier = appName
                }));

            lifeTime.ApplicationStopped.Register(() =>
                SystemEvents.Application.ApplicationStopped(new ApplicationStopEventArgs
                {
                    Services = app.ApplicationServices,
                    AppIdentifier = appName
                }));
            return lifeTime;
        }
    }
}

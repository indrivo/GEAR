using Microsoft.AspNetCore.Builder;
using GR.Core.Events;
using GR.Core.Events.EventArgs;
using Microsoft.Extensions.Hosting;

namespace GR.Core.Extensions
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

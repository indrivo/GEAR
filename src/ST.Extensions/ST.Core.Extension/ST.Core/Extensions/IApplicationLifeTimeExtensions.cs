using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using ST.Core.Events;
using ST.Core.Events.EventArgs;

namespace ST.Core.Extensions
{
    public static class ApplicationLifeTimeExtensions
    {
        public static IApplicationLifetime RegisterAppEvents(this IApplicationLifetime lifeTime, IApplicationBuilder app, string appName)
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

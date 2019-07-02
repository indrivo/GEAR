using Microsoft.AspNetCore.Hosting;
using ST.Core.Events;
using ST.Core.Events.EventArgs;

namespace ST.Core.Extensions
{
    public static class ApplicationLifeTimeExtensions
    {
        public static IApplicationLifetime RegisterAppEvents(this IApplicationLifetime lifeTime, string appName)
        {
            lifeTime.ApplicationStarted.Register(() =>
                SystemEvents.Application.ApplicationStarted(new ApplicationStartedEventArgs
                {
                    AppIdentifier = appName
                }));

            lifeTime.ApplicationStopped.Register(() =>
                SystemEvents.Application.ApplicationStopped(new ApplicationStopEventArgs()
                {
                    AppIdentifier = appName
                }));
            return lifeTime;
        }
    }
}

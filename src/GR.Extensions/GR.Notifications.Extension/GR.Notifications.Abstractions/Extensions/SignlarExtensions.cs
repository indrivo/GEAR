using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Notifications.Abstractions.ServiceBuilder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Notifications.Abstractions.Extensions
{
    public static class SignlaRExtensions
    {
        /// <summary>
        /// Use signalR from GR.Notifications By Indrivo
        /// </summary>
        /// <param name="app"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseNotificationsHub<THub>(this IApplicationBuilder app, string path = "/rtn")
            where THub : Hub
        {
            app.UseSignalR(routes =>
            {
                routes.MapHub<THub>(path,
                    options =>
                    {
                        options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransports.All;
                    });
            });
            return app;
        }

        /// <summary>
        /// Add SignalR
        /// </summary>
        /// <param name="services"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static INotificationServiceCollection RegisterNotificationsHubModule<TCommunicationHub>(this INotificationServiceCollection services, string path = "/rtn")
        where TCommunicationHub : class, ICommunicationHub
        {
            Arg.NotNull(services, nameof(services));
            services.Services.AddGearSingleton<ICommunicationHub, TCommunicationHub>();
            services.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
            services.Services.AddHealthChecks()
                .AddSignalRHub(GearApplication.SystemConfig.EntryUri + path.Substring(1), "signalr-hub");
            return services;
        }
    }
}
using GR.Core.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Notifications.Abstractions.Extensions
{
    public static class SignlarExtensions
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
        /// <returns></returns>
        public static IServiceCollection RegisterNotificationsHubModule<TCommunicationHub>(this IServiceCollection services)
        where TCommunicationHub : class, ICommunicationHub
        {
            Arg.NotNull(services, nameof(services));
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
            services.AddTransient<ICommunicationHub, TCommunicationHub>();
            return services;
        }
    }
}
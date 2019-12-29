using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using GR.Core.Helpers;
using GR.Notifications.Abstractions;
using GR.Notifications.Hubs;

namespace GR.Notifications.Extensions
{
    public static class SignlarExtensions
    {
        /// <summary>
        /// Use signalR from GR.Notifications By Indrivo
        /// </summary>
        /// <param name="app"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSignalRModule(this IApplicationBuilder app, string path = "/rtn")
        {
            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalRNotificationHub>(path,
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
        public static IServiceCollection AddSignalRModule(this IServiceCollection services) 
        {
            Arg.NotNull(services, nameof(services));
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
            services.AddTransient<INotificationHub, LocalNotificationHub>();
            return services;
        }
    }
}

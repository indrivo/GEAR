using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using GR.Core.Helpers;
using GR.Notifications.Abstractions;
using GR.Notifications.Hubs;
using GR.Notifications.Services;

namespace GR.Notifications.Extensions
{
    public static class SignlarExtensions
    {
        /// <summary>
        /// Use signalR from GR.Notifications By Indrivo
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSignalRModule(this IApplicationBuilder app)
        {
            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalRNotificationHub>("/rtn",
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
        public static IServiceCollection AddSignalRModule<TContext, TUser, TRole>(this IServiceCollection services) where TContext : IdentityDbContext<TUser, TRole, string>
            where TUser : IdentityUser where TRole : IdentityRole<string>
        {
            Arg.NotNull(services, nameof(services));
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
            services.AddTransient<INotificationHub, LocalNotificationHub>();
            services.AddTransient<INotify<TRole>, Notify<TContext, TRole, TUser>>();
            return services;
        }
    }
}

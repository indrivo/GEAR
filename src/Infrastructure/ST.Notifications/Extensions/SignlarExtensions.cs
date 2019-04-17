using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using ST.Notifications.Abstractions;
using ST.Notifications.Hubs;
using ST.Notifications.Services;

namespace ST.Notifications.Extensions
{
    public static class SignlarExtensions
    {
        /// <summary>
        /// Use signalR from ST.Notifications By Soft-Tehnica
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSignalR(this IApplicationBuilder app)
        {
            app.UseSignalR(routes =>
            {
                routes.MapHub<NotificationsHub>("/rtn",
                    options =>
                    {
                        options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransports.All;
                    });
            });
            return app;
        }
        /// <summary>
        /// Add SignalR from ST.Notifications By Soft-Tehnica
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSignalR<TContext, TUser, TRole>(this IServiceCollection services) where TContext : IdentityDbContext<TUser, TRole, string>
            where TUser : IdentityUser where TRole : IdentityRole<string>
        {
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
            services.AddTransient<INotificationHub, NotificationProvider<TUser>>();
            services.AddTransient<INotify<TRole>, Notify<TContext, TRole, TUser>>();
            return services;
        }
    }
}

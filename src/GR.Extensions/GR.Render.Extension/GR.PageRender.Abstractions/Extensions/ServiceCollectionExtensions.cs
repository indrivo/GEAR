using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GR.Audit.Abstractions.Extensions;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.PageRender.Abstractions.Events;
using GR.PageRender.Abstractions.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace GR.PageRender.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add page module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPageModule(this IServiceCollection services)
        {
            Arg.NotNull(services, nameof(services));
            DynamicUiEvents.RegisterEvents();
            return services;
        }

        /// <summary>
        /// Add module storage
        /// </summary>
        /// <typeparam name="TPageContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddPageModuleStorage<TPageContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
            where TPageContext : DbContext, IDynamicPagesContext
        {
            services.AddScopedContextFactory<IDynamicPagesContext, TPageContext>();
            services.AddDbContext<TPageContext>(options);
            services.RegisterAuditFor<IDynamicPagesContext>("Page module");

            SystemEvents.Database.OnMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TPageContext>();
            };
            return services;
        }

        /// <summary>
        /// Register view mode service
        /// </summary>
        /// <typeparam name="TViewModelService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterViewModelService<TViewModelService>(this IServiceCollection services)
            where TViewModelService : class, IViewModelService
        {
            services.AddGearTransient<IViewModelService, TViewModelService>();
            return services;
        }

        /// <summary>
        /// Use custom url rewrite module
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static void UseUrlRewriteModule(this IApplicationBuilder app)
            => app.Use(async (ctx, next) =>
            {
                try
                {
                    if (GearApplication.Configured) await ctx.ConfiguredSystem(next);
                    else await ctx.NonConfiguredSystem(next);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });


        /// <summary>
        /// On non configured system action
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="next"></param>
        public static async Task NonConfiguredSystem(this HttpContext ctx, Func<Task> next)
        {
            if (ctx.Request.Cookies.Count >= 2) ctx.DeleteCookies();
            if (ctx.Request.Path.Value != "/"
                && UrlRewriteHelper.IsNotSystemRoute(ctx.Request.Path.Value)
                && !ctx.Request.Path.Value.ToLowerInvariant().StartsWith("/installer", StringComparison.Ordinal))
            {
                var originalPath = ctx.Request.Path.Value;
                ctx.Items["originalPath"] = originalPath;
                ctx.Request.Path = "/Installer";
                await next();
            }
            else
                await next();
        }

        /// <summary>
        /// On configured system
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public static async Task ConfiguredSystem(this HttpContext ctx, Func<Task> next)
        {
            try
            {
                UrlRewriteHelper.CheckLanguage(ref ctx);
                await next();
                var isClientUrl = await UrlRewriteHelper.ParseClientRequestAsync(ctx);
                if (isClientUrl) await next();
                else if (ctx.Response.StatusCode == StatusCodes.Status404NotFound && !ctx.Response.HasStarted)
                {
                    //Re-execute the request so the user gets the error page
                    var originalPath = ctx.Request.Path.Value;
                    ctx.Items["originalPath"] = originalPath;
                    ctx.Request.Path = "/Handler/NotFound";
                    await next();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

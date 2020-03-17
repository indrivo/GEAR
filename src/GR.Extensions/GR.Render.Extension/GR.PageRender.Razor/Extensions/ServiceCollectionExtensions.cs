using System;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using GR.PageRender.Abstractions;
using GR.PageRender.Razor.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace GR.PageRender.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register razor page render
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPageRenderUIModule<TPageRenderService>(this IServiceCollection services)
            where TPageRenderService : class, IPageRender
        {
            services.AddTransient<IPageRender, TPageRenderService>();
            services.ConfigureOptions(typeof(PageRenderFileConfiguration));
            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
                    {
                        await args.InjectService<IMenuService>().AppendMenuItemsAsync(new PagesMenuInitializer());
                    });
            };
            return services;
        }

        /// <summary>
        /// Add page acl
        /// </summary>
        /// <typeparam name="TPageAclService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPageAclService<TPageAclService>(this IServiceCollection services)
            where TPageAclService : class, IPageAclService
        {
            services.AddTransient<IPageAclService, TPageAclService>();

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
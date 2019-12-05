using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GR.Core;
using GR.Identity.Abstractions;
using GR.Identity.Data;
using GR.PageRender.Abstractions;
using GR.PageRender.Abstractions.Models.Pages;

namespace GR.Application.Middleware.Server
{
    public static class UrlRewrite
    {
        /// <summary>
        /// On non configured system action
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="next"></param>
        private static async Task NonConfiguredSystem(this HttpContext ctx, Func<Task> next)
        {
            if (ctx.Request.Cookies.Count >= 2) ctx.DeleteCookies();
            if (ctx.Request.Path.Value != "/"
                && ExcludeAssets(ctx.Request.Path.Value)
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
        private static async Task ConfiguredSystem(this HttpContext ctx, Func<Task> next)
        {
            try
            {
                CheckLanguage(ref ctx);
                await next();
                var isClientUrl = ctx.ParseClientRequest();
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

        /// <summary>
        /// Check language
        /// </summary>
        /// <param name="ctx"></param>
        private static void CheckLanguage(ref HttpContext ctx)
        {
            try
            {
                if (ctx.Request.Cookies.FirstOrDefault(x => x.Key == "language").Equals(default(KeyValuePair<string, string>)))
                {
                    ctx.Response.Cookies.Append("language", GearSettings.DEFAULT_LANGUAGE);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Use custom url rewrite module
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static void UseUrlRewriteModule(this IApplicationBuilder app)
        {
            app.Use(async (ctx, next) =>
            {
                try
                {
                    var env = ctx.RequestServices.GetRequiredService<IConfiguration>();
                    var isConfigured = env.GetValue<bool>("IsConfigured");
                    if (isConfigured)
                        await ctx.ConfiguredSystem(next);
                    else await ctx.NonConfiguredSystem(next);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        /// <summary>
        /// Delete cookies
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private static void DeleteCookies(this HttpContext ctx)
        {
            foreach (var cookie in ctx.Request.Cookies.Keys)
            {
                ctx.Response.Cookies.Delete(cookie);
            }
        }

        /// <summary>
        /// Exclude assets
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool ExcludeAssets(string value)
        {
            return !value.StartsWith("/css", StringComparison.Ordinal)
                && !value.StartsWith("/lib", StringComparison.Ordinal)
                && !value.StartsWith("/assets", StringComparison.Ordinal)
                && !value.StartsWith("/js", StringComparison.Ordinal)
                && !value.StartsWith("/themes", StringComparison.Ordinal)
                && !value.StartsWith("/PageRender", StringComparison.Ordinal)
                && !value.StartsWith("/Localization", StringComparison.Ordinal)
                && !value.StartsWith("/favicon.ico", StringComparison.Ordinal)
                && !value.StartsWith("/images", StringComparison.Ordinal);
        }

        /// <summary>
        /// HttpContext extension
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private static bool ParseClientRequest(this HttpContext ctx)
        {
            var context = ctx.RequestServices.GetRequiredService<IDynamicPagesContext>();
            var parameters = HttpUtility.ParseQueryString(ctx.Request.QueryString.ToString());
            var originalPath = ctx.Request.Path.Value;
            ctx.Items["originalPath"] = originalPath;

            var (match, page) = Match(context, originalPath, parameters);

            if (!match) return false;
            ctx.Request.Path = "/PageRender";

            var newParams = HttpUtility.ParseQueryString($"pageId={page.Id}");

            ctx.Request.QueryString = QueryString.Create(newParams.ToKeyValuePair());

            return true;
        }

        /// <summary>
        /// Match routes
        /// </summary>
        /// <param name="context"></param>
        /// <param name="path"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static (bool, Page) Match(IDynamicPagesContext context, string path, NameValueCollection parameters)
        {
            var route = context.Pages.Where(x => !x.IsLayout && !x.IsDeleted).ToList().FirstOrDefault(x => RouteFinder(
                new ValueTuple<Page, string, List<KeyValuePair<string, string>>>
                    (x, path, parameters.ToKeyValuePair().ToList())));

            return route == null ? default : (true, route);
        }

        /// <summary>
        /// Route Match
        /// </summary>
        private static readonly Func<(Page, string, List<KeyValuePair<string, string>>), bool> RouteFinder =
            data => data.Item1.Path.ToLower().Equals(data.Item2.ToLower()) && !data.Item2.Equals("/");

        /// <summary>
        /// NameValueCollection to KeyValuePair
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<string, string>> ToKeyValuePair(this NameValueCollection collection)
        {
            return collection.AllKeys.Select(x => new KeyValuePair<string, string>(x, collection[x]));
        }

        /// <summary>
        /// Claims Synchronizer
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseClaimsSynchronizer<TContext>(this IApplicationBuilder app) where TContext : ApplicationDbContext
        {
            app.Use(async (ctx, next) =>
            {
                await next();
                if (ctx.Response.StatusCode == StatusCodes.Status302Found && !ctx.Response.HasStarted)
                {
                    //Get userName of user
                    var oid = ctx.User.Identity.Name;
                    if (oid != null)
                    {
                        var context = ctx.RequestServices.GetRequiredService<TContext>();
                        //Get signInManager
                        var signInManager = ctx.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
                        var user = context.Users.FirstOrDefault(x => x.UserName.Equals(oid));
                        await signInManager.RefreshSignInAsync(user);
                    }
                    await next();
                }
            });

            return app;
        }
    }
}

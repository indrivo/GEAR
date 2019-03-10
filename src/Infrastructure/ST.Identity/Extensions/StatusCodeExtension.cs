using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ST.Entities.Data;
using ST.Entities.Models.Pages;
using ST.Identity.Data;
using ST.Identity.Data.UserProfiles;

namespace ST.Identity.Extensions
{
    public static class StatusCodeExtension
    {
        /// <summary>
        /// Use custom error pages
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UsePageRedirect(this IApplicationBuilder app)
        {
            app.Use(async (ctx, next) =>
            {
                var env = ctx.RequestServices.GetRequiredService<IConfiguration>();
                var isConfigured = env.GetValue<bool>("IsConfigurated");
                if (!isConfigured)
                {
                    if (ctx.Request.Cookies.Count >= 2)
                    {
                        foreach (var cookie in ctx.Request.Cookies.Keys)
                        {
                            //if (cookie != ".ST.CORE.Data")
                            ctx.Response.Cookies.Delete(cookie);
                        }
                    }
                    if (ctx.Request.Path.Value != "/"
                    && ExcludeAssets(ctx.Request.Path.Value)
                    && !(ctx.Request.Path.Value.ToString().ToLower().StartsWith("/installer")))
                    {
                        var originalPath = ctx.Request.Path.Value;
                        ctx.Items["originalPath"] = originalPath;
                        ctx.Request.Path = "/Installer";
                        await next();
                    }
                    else
                        await next();
                }
                else
                {
                    await next();
                    var isClientUrl = ctx.ParseClientRequest();
                    if (isClientUrl) await next();
                    else if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
                    {
                        //Re-execute the request so the user gets the error page
                        var originalPath = ctx.Request.Path.Value;
                        ctx.Items["originalPath"] = originalPath;
                        ctx.Request.Path = "/Handler/NotFound";
                        await next();
                    }
                }
            });

            return app;
        }

        /// <summary>
        /// Exclude assets
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ExcludeAssets(string value)
        {
            return !value.StartsWith("/css")
                && !value.StartsWith("/lib")
                && !value.StartsWith("/assets")
                && !value.StartsWith("/js")
                && !value.StartsWith("/PageRender")
                && !value.StartsWith("/Localization")
                && !value.StartsWith("/favicon.ico")
                && !value.StartsWith("/images");
        }

        /// <summary>
        /// HttpContext extension
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private static bool ParseClientRequest(this HttpContext ctx)
        {
            var context = ctx.RequestServices.GetRequiredService<EntitiesDbContext>();
            var parameters = HttpUtility.ParseQueryString(ctx.Request.QueryString.ToString());
            var originalPath = ctx.Request.Path.Value;
            ctx.Items["originalPath"] = originalPath;

            var match = Match(context, originalPath, parameters);

            if (!match.Item1) return false;
            ctx.Request.Path = "/PageRender";

            var newParams = HttpUtility.ParseQueryString($"pageId={match.Item2}");

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
        private static (bool, Guid) Match(EntitiesDbContext context, string path, NameValueCollection parameters)
        {
            var route = context.Pages.Where(x => !x.IsLayout && !x.IsDeleted).ToList().FirstOrDefault(x => RouteFinder(
                new ValueTuple<Page, string, List<KeyValuePair<string, string>>>
                    (x, path, parameters.ToKeyValuePair().ToList())));

            return route == null ? default : (true, route.Id);
        }

        /// <summary>
        /// Route Match
        /// </summary>
        private static readonly Func<(Page, string, List<KeyValuePair<string, string>>), bool> RouteFinder =
            delegate ((Page, string, List<KeyValuePair<string, string>>) data)
            {
                var search = data.Item2.Split("/");
                //if (data.Item1.Path.StartsWith("/" + search[1]))
                //{
                //    return true;
                //}

                return data.Item1.Path.ToLower().StartsWith(data.Item2.ToLower()) && !data.Item2.Equals("/");
            };

        /// <summary>
        /// NameValueCollection to KeyValuePair
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePair(this NameValueCollection collection)
        {
            return collection.AllKeys.Select(x => new KeyValuePair<string, string>(x, collection[x]));
        }

        /// <summary>
        /// Claims Synchronizer
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseClaimsSynchronizer(this IApplicationBuilder app)
        {
            app.Use(async (ctx, next) =>
            {
                await next();
                if (ctx.Response.StatusCode == 302 && !ctx.Response.HasStarted)
                {
                    //Get userName of user
                    var oid = ctx.User.Identity.Name;
                    if (oid != null)
                    {
                        var context = ctx.RequestServices.GetRequiredService<ApplicationDbContext>();
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

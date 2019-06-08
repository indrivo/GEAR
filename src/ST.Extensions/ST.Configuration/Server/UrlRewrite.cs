using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ST.Cache.Abstractions;
using ST.Core;
using ST.Core.Extensions;
using ST.Entities.Data;
using ST.Entities.Models.Pages;
using ST.Identity.Abstractions;
using ST.Identity.Data;

namespace ST.Configuration.Server
{
    public static class UrlRewrite
    {
        /// <summary>
        /// On non configured system action
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="next"></param>
        private static async Task OnNonConfiguredSystem(this HttpContext ctx, Func<Task> next)
        {
            if (ctx.Request.Cookies.Count >= 2)
            {
                ctx.DeleteCookies();
            }
            if (ctx.Request.Path.Value != "/"
                && ExcludeAssets(ctx.Request.Path.Value)
                && !(ctx.Request.Path.Value.ToLower().StartsWith("/installer")))
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
        private static async Task OnConfiguredSystem(this HttpContext ctx, Func<Task> next)
        {
            CheckLanguage(ref ctx);
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

        /// <summary>
        /// Check tenant
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private static async Task CheckTenant(this HttpContext ctx)
        {
            var tenantId = ctx.User?.Claims?.FirstOrDefault(x => x.Type == "tenant")?.Value?.ToGuid();
            if (tenantId == Guid.Empty || tenantId == null)
            {
                try
                {
                    var userManager = ctx.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                    var user = userManager.GetUserAsync(ctx.User).GetAwaiter().GetResult();
                    if (user != null)
                    {
                        var claim = new Claim("tenant", user.TenantId.ToString());
                        //await userManager.RemoveClaimAsync(user, claim);
                        await userManager.AddClaimAsync(user, claim);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
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
                    ctx.Response.Cookies.Append("language", Settings.DefaultLanguage);
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
                var env = ctx.RequestServices.GetRequiredService<IConfiguration>();
                var isConfigured = env.GetValue<bool>("IsConfigured");
                if (isConfigured)
                    await ctx.OnConfiguredSystem(next);
                else await ctx.OnNonConfiguredSystem(next);
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

            var (match, pageId) = Match(context, originalPath, parameters);

            if (!match) return false;
            ctx.Request.Path = "/PageRender";

            var newParams = HttpUtility.ParseQueryString($"pageId={pageId}");

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

                return data.Item1.Path.ToLower().Equals(data.Item2.ToLower()) && !data.Item2.Equals("/");
            };

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
                if (ctx.Response.StatusCode == 302 && !ctx.Response.HasStarted)
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

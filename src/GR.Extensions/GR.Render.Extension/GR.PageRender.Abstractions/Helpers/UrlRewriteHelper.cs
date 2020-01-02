using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.PageRender.Abstractions.Constants;
using GR.PageRender.Abstractions.Models.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable 1998

namespace GR.PageRender.Abstractions.Helpers
{
    public static class UrlRewriteHelper
    {
        /// <summary>
        /// Excluded routes
        /// </summary>
        private static readonly List<string> ExcludedRoutes = new List<string>
        {
            "/css",
            "/lib",
            "/assets",
            "/js",
            "/themes",
            "/PageRender",
            "/Localization",
            "/favicon",
            "/images",
            "/rtn"
        };

        /// <summary>
        /// Exclude assets
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool IsNotSystemRoute(string value) => ExcludedRoutes.All(x => !value.StartsWith(x, StringComparison.Ordinal));

        /// <summary>
        /// Route Match
        /// </summary>
        private static readonly Func<(Page, string, IEnumerable<KeyValuePair<string, string>>), bool> RouteFinder =
            data => data.Item1.Path.ToLowerInvariant().Equals(data.Item2.ToLowerInvariant()) && !data.Item2.Equals("/");

        /// <summary>
        /// Check language
        /// </summary>
        /// <param name="ctx"></param>
        internal static void CheckLanguage(ref HttpContext ctx)
        {
            if (ctx.Request.Cookies.FirstOrDefault(x => x.Key == "language").IsNull())
                ctx.Response.Cookies.Append("language", GearSettings.DEFAULT_LANGUAGE);
        }

        /// <summary>
        /// HttpContext extension
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        internal static async Task<bool> ParseClientRequestAsync(HttpContext ctx)
        {
            var parameters = HttpUtility.ParseQueryString(ctx.Request.QueryString.ToString());
            var originalPath = ctx.Request.Path.Value;
            ctx.Items["originalPath"] = originalPath;

            if (!IsNotSystemRoute(originalPath)) return false;
            var contains = AppRoutes.RegisteredRoutes.AnyStartWith(originalPath);
            if (contains) return false;
            var (match, page) = await MatchAsync(ctx, originalPath, parameters);

            if (!match) return false;
            ctx.Request.Path = "/PageRender";

            var newParams = HttpUtility.ParseQueryString($"pageId={page.Id}");

            ctx.Request.QueryString = QueryString.Create(newParams.ToKeyValuePair());

            return true;
        }

        /// <summary>
        /// Match routes
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="path"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static async Task<(bool, Page)> MatchAsync(HttpContext ctx, string path, NameValueCollection parameters)
        {
            var context = ctx.RequestServices.GetRequiredService<IDynamicPagesContext>();
            var memoryCache = ctx.RequestServices.GetRequiredService<IMemoryCache>();
            var key = $"route_{path.ToLowerInvariant()}";
            var cachePageId = memoryCache.Get<Guid?>(key);
            if (cachePageId.HasValue)
            {
                var cachePage = memoryCache.Get<Page>($"{PageRenderConstants.PageCacheIdentifier}{cachePageId.Value}");
                return (cachePage != null, cachePage);
            }

            var page = context.Pages.Where(x => !x.IsLayout && !x.IsDeleted).ToList().FirstOrDefault(x => RouteFinder(
               new ValueTuple<Page, string, IEnumerable<KeyValuePair<string, string>>>
                   (x, path.ToLowerInvariant(), parameters.ToKeyValuePair())));

            if (page != null) memoryCache.Set(key, page.Id);
            return (page != null, page);
        }
    }
}

using System;
using Microsoft.AspNetCore.Http;

namespace GR.Core.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string RequestedWithHeader = "X-Requested-With";
        private const string XmlHttpRequest = "XMLHttpRequest";

        /// <summary>
        /// Check if is ajax request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
		public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return request.Headers[RequestedWithHeader] == XmlHttpRequest;

            return false;
        }

        /// <summary>
        /// Get app base url
        /// </summary>
        /// <param name="accessor"></param>
        /// <returns></returns>
        public static string GetAppBaseUrl(this IHttpContextAccessor accessor)
        {
            var request = accessor?.HttpContext?.Request;
            return $"{request?.Scheme}://{request?.Host}{request?.PathBase}";
        }

        /// <summary>
        /// Get app url
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetAppBaseUrl(this HttpContext context)
        {
            var request = context?.Request;
            return $"{request?.Scheme}://{request?.Host}{request?.PathBase}";
        }


        /// <summary>
        /// Delete cookies
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static void DeleteCookies(this HttpContext ctx)
        {
            foreach (var cookie in ctx.Request.Cookies.Keys)
            {
                ctx.Response.Cookies.Delete(cookie);
            }
        }
    }
}
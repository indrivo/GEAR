using System.Linq;
using Microsoft.AspNetCore.Http;

namespace GR.Identity.Abstractions.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Check if is bearer request
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsBearerRequest(this HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            return authHeader?.StartsWith("Bearer ") ?? false;
        }
    }
}
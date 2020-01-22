using System;
using GR.Core.Helpers;
using Microsoft.AspNetCore.Http;

namespace GR.WebApplication.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Map route
        /// </summary>
        /// <param name="context"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static HttpContext MapTo(this HttpContext context, string path)
        {
            Arg.NotNullOrEmpty(path, nameof(MapTo));
            if (!path.StartsWith("/")) throw new Exception("Path need to start with / symbol and it is an existent route");
            var originalPath = context.Request.Path.Value;
            context.Items["originalPath"] = originalPath;
            context.Request.Path = path;
            return context;
        }
    }
}

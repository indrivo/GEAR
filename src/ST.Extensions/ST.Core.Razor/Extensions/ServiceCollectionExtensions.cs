using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
#pragma warning disable 1998

namespace ST.Core.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Use app as mvc
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <param name="routeMapping"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAppMvc(this IApplicationBuilder app, IConfiguration configuration, Dictionary<string, Action<HttpContext>> routeMapping = null)
        {
            var isConfigured = configuration.GetValue<bool>("IsConfigured");

            var singleTenantTemplate = isConfigured
                ? "{controller=Home}/{action=Index}"
                : "{controller=Installer}/{action=Index}";

            app.UseMvc(routes =>
            {
                if (routeMapping != null)
                {
                    foreach (var map in routeMapping)
                    {
                        routes.MapGet(map.Key, async context => map.Value.Invoke(context));
                    }
                }
                routes.MapRoute(
                    name: "default",
                    template: singleTenantTemplate,
                    defaults: singleTenantTemplate
                //constraints: new { tenant = new TenantRouteConstraint() }
                );
            });
            return app;
        }
    }
}

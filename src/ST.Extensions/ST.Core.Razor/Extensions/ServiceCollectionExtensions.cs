using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ST.Core.Extensions;

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
                routes.ApplicationBuilder.Use(async (context, next) =>
                {
                    if (routeMapping == null || !isConfigured)
                    {
                        await next();
                    }
                    else
                    {
                        var match = routeMapping.FirstOrDefault(o => o.Key.Equals(context.Request.Path));
                        if (!match.IsNull())
                        {
                            match.Value.Invoke(context);
                            await next();
                        }
                        else
                        {
                            await next();
                        }
                    }
                });

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

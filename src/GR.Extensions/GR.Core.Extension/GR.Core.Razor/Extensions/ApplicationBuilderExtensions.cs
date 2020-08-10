using System.Threading.Tasks;
using GR.Core.Extensions;
using Microsoft.AspNetCore.Builder;

namespace GR.Core.Razor.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Use gear status code response
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseGearStatusCodeResponse(this IApplicationBuilder app)
        {
            app.UseStatusCodePages(context =>
              {
                  if (context.HttpContext.Request.IsAjaxRequest() || context.HttpContext.Request.IsApiRequest())
                      return Task.CompletedTask;

                  switch (context.HttpContext.Response.StatusCode)
                  {
                      case 404:
                          if (!context.HttpContext.Request.Path.Value.EndsWith(".map"))
                          {
                              context.HttpContext.Response.Redirect("/Handler/NotFound");
                          }
                          break;
                      case 403:
                          context.HttpContext.Response.Redirect("/Account/AccessDenied");
                          break;
                  }

                  return Task.CompletedTask;
              });

            return app;
        }
    }
}

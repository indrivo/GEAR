using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Razor.BaseControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GR.Core.Razor.Helpers.Filters
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class JsonApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// On api exception occured
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JsonApiExceptionFilterAttribute>>();
            logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            var result = new ResultModel
            {
                IsSuccess = false
            };

            var exception = context.Exception;

            result.Errors = !string.IsNullOrEmpty(exception.Message)
                ? new List<IErrorModel> { new ErrorModel("API_UnhandledException", exception.Message) }
                : new List<IErrorModel> { new ErrorModel("API_UnhandledException", "An unhandled exception has occurred.") };

            var serializeObject = JsonConvert.SerializeObject(result);

            context.HttpContext.Response.ContentType = ContentType.ApplicationJson;
            await context.HttpContext.Response.WriteAsync(serializeObject);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            context.ExceptionHandled = true;
        }
    }
}

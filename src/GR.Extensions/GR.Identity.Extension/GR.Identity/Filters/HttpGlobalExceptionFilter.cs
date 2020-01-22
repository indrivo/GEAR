using GR.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace GR.Identity.Filters
{
    /// <inheritdoc />
    /// <summary>
    ///     A filter that runs after an action has thrown an <see cref="T:System.Exception" />. It works at global level.
    /// </summary>
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        /// <summary>
        ///     The instance of <see cref="HttpGlobalExceptionFilter" />
        /// </summary>
        public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called after an action has thrown an <see cref="T:System.Exception" />.
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            var result = new ResultModel
            {
                IsSuccess = false
            };

            var exception = context.Exception;

            result.Errors = !string.IsNullOrEmpty(exception.Message)
                ? new List<IErrorModel> { new ErrorModel(ExceptionCodes.UnhandledException, exception.Message) }
                : new List<IErrorModel>
                {
                    new ErrorModel(ExceptionCodes.UnhandledException, "An unhandled exception has occurred.")
                };

            var serializeObject = JsonConvert.SerializeObject(result);

            context.Result = new BadRequestObjectResult(serializeObject);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.ExceptionHandled = true;
        }
    }
}
using GR.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Newtonsoft.Json;

namespace GR.Identity.Versioning
{
    /// <inheritdoc />
    /// <summary>
    /// Represents the default implementation for creating HTTP error responses related to API versioning.
    /// </summary>
    public class UnsupportedApiVersionErrorResponseProvider : DefaultErrorResponseProvider
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates and returns a new error response given the provided context.
        /// </summary>
        public override IActionResult CreateResponse(ErrorResponseContext context)
        {
            var resultModel = new ResultModel
            {
                IsSuccess = false,
                Errors = new[] { new ErrorModel($"API_{context.ErrorCode}", context.Message) },
                Result = new object()
            };

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(resultModel),
                ContentType = "application/json"
            };
        }
    }
}
using GR.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Linq;
using System.Net;

namespace GR.Identity.Filters
{
    /// <inheritdoc />
    /// <summary>
    ///     Filter of <see cref="T:Microsoft.AspNetCore.Mvc.Filters.IActionFilter" /> type used to validate Controller MVC
    ///     Model State
    /// </summary>
    public sealed class ModelValidationFilter : IActionFilter
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public ModelValidationFilter()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        /// <inheritdoc />
        /// <summary>
        ///     Trigger the filter OnActionExecuting
        /// </summary>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
                return;

            var result = new ResultModel
            {
                IsSuccess = false,
                Errors = context.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Where(x => x != null)
                    .Select(x => x.ToErrorModel())
                    .ToArray()
            };

            context.Result = new ContentResult
            {
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(result, _serializerSettings),
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        /// <inheritdoc />
        /// <summary>
        ///     Trigger the filter OnActionExecuted
        /// </summary>
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
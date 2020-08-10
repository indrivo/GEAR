using GR.Core.Extensions;
using GR.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GR.Core.Razor.Helpers.Filters
{
    /// <summary>
    /// Validate model state and return errors if an error occurred
    /// </summary>
    public class ApiValidationActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid && (context.HttpContext.Request.IsAjaxRequest() || context.HttpContext.Request.IsApiRequest()))
            {
                context.Result = new JsonResult(new ResultModel().AttachModelState(context.ModelState));
            }

            base.OnActionExecuting(context);
        }
    }
}

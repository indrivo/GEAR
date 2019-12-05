using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;
using GR.Core.Extensions;

namespace GR.Core.Attributes
{
	/// <inheritdoc />
    /// <summary>
    /// Check if is a ajax request
    /// </summary>
	public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
	{
		/// <inheritdoc />
        /// <summary>
        /// Check ajax
        /// </summary>
        /// <param name="routeContext"></param>
        /// <param name="action"></param>
        /// <returns></returns>
		public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
		{
			return routeContext.HttpContext.Request.IsAjaxRequest();
		}
	}
}
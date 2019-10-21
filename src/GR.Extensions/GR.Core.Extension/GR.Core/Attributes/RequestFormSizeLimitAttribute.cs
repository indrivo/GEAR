using System;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GR.Core.Attributes
{
	/// <summary>
	/// Filter to set size limits for request form data
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class RequestFormSizeLimitAttribute : Attribute, IAuthorizationFilter, IOrderedFilter
	{
		private readonly FormOptions _formOptions;

		public RequestFormSizeLimitAttribute(int valueCountLimit)
		{
			_formOptions = new FormOptions
			{
				ValueCountLimit = valueCountLimit
			};
		}

		public int Order { get; set; }

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var features = context.HttpContext.Features;
			var formFeature = features.Get<IFormFeature>();

			if (formFeature?.Form == null)
			{
				// Request form has not been read yet, so set the limits
				features.Set<IFormFeature>(new FormFeature(context.HttpContext.Request, _formOptions));
			}
		}
	}
}
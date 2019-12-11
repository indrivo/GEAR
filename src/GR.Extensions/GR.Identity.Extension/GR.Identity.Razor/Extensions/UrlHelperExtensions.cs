using Microsoft.AspNetCore.Mvc;
using GR.Identity.Razor.Controllers;

namespace GR.Identity.Razor.Extensions
{
	public static class UrlHelperExtensions
	{
		public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
		{
			return urlHelper.Action(
				nameof(AccountController.ResetPassword),
				"Account",
				new { userId, code },
				scheme);
		}
	}
}
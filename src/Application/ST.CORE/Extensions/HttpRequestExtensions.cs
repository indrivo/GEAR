using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace ST.CORE.Extensions
{
	public static class HttpRequestExtensions
	{
		private const string RequestedWithHeader = "X-Requested-With";
		private const string XmlHttpRequest = "XMLHttpRequest";

		public static bool IsAjaxRequest(this HttpRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			if (request.Headers != null)
				return request.Headers[RequestedWithHeader] == XmlHttpRequest;

			return false;
		}
	}
}
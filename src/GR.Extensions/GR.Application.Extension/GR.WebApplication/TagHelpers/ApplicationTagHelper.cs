using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.PageRender.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GR.WebApplication.TagHelpers
{
	[HtmlTargetElement("App")]
	public class ApplicationTagHelper : TagHelper
	{
		/// <summary>
		/// Inject page render
		/// </summary>
		private readonly IPageRender _pageRender;

		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public ApplicationTagHelper(IPageRender pageRender)
		{
			_pageRender = pageRender;
		}

		/// <summary>
		/// Layout
		/// </summary>
		public Guid? LayoutId { get; set; }


		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "div";
			var initial = (await output.GetChildContentAsync()).GetContent();
			var (pre, next) = await _pageRender.GetLayoutHtml(LayoutId);
			var pageTitle = string.Empty;
			if (ViewContext?.ViewData?.ContainsKey("Title") ?? false)
			{
				pageTitle = ViewContext.ViewData["Title"].ToString();
			}
			var dictData = new Dictionary<string, string>
			{
				{ "PageTitle", pageTitle }
			};
			var content = new StringBuilder();
			content.Append(pre.Value.Inject(dictData));
			content.Append(initial);
			content.Append(next.Value.Inject(dictData));
			output.Content.SetHtmlContent(content.ToString());
		}
	}
}

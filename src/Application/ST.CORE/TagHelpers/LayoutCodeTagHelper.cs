using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ST.CORE.Services.Abstraction;

namespace ST.CORE.TagHelpers
{
	[HtmlTargetElement("layout-css-code")]
	public class LayoutCssCodeTagHelper : TagHelper
	{
		/// <summary>
		/// Inject page render
		/// </summary>
		private readonly IPageRender _pageRender;

		public LayoutCssCodeTagHelper(IPageRender pageRender)
		{
			_pageRender = pageRender;
		}
		/// <summary>
		/// Layout
		/// </summary>
		public Guid? LayoutId { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "style";
			var cssCode = _pageRender.GetLayoutCss(LayoutId);

			output.Content.SetHtmlContent(cssCode);
		}
	}

	[HtmlTargetElement("layout-js-code")]
	public class LayoutJsCodeTagHelper : TagHelper
	{
		/// <summary>
		/// Inject page render
		/// </summary>
		private readonly IPageRender _pageRender;

		public LayoutJsCodeTagHelper(IPageRender pageRender)
		{
			_pageRender = pageRender;
		}
		/// <summary>
		/// Layout
		/// </summary>
		public Guid? LayoutId { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "script";
			var cssCode = _pageRender.GetLayoutJavaScript(LayoutId);

			output.Content.SetHtmlContent(cssCode);
		}
	}
}

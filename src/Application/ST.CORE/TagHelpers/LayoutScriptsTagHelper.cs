using Microsoft.AspNetCore.Razor.TagHelpers;
using ST.CORE.Services.Abstraction;
using System;
using System.Text;

namespace ST.CORE.TagHelpers
{
	[HtmlTargetElement("application-styles")]
	public class LayoutJsScriptsTagHelper : TagHelper
	{

		/// <summary>
		/// Inject page render
		/// </summary>
		private readonly IPageRender _pageRender;

		public LayoutJsScriptsTagHelper(IPageRender pageRender)
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
			var content = new StringBuilder();
			if (LayoutId == null)
			{
				output.Content.SetHtmlContent(string.Empty);
				return;
			}

			var styles = _pageRender.GetPageStyles(LayoutId.Value);
			if (styles.IsSuccess)
			{
				foreach (var style in styles.Result)
				{
					content.AppendFormat(style.Script);
				}
			}
			output.Content.SetHtmlContent(content.ToString());
		}
	}

	[HtmlTargetElement("application-scripts")]
	public class LayoutCssScriptsTagHelper : TagHelper
	{

		/// <summary>
		/// Inject page render
		/// </summary>
		private readonly IPageRender _pageRender;

		public LayoutCssScriptsTagHelper(IPageRender pageRender)
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
			if (LayoutId == null)
			{
				output.Content.SetHtmlContent(string.Empty);
				return;
			}
			var scripts = _pageRender.GetPageScripts(LayoutId.Value);
			var content = new StringBuilder();
			if (scripts.IsSuccess)
			{
				foreach (var script in scripts.Result)
				{
					content.AppendFormat(script.Script);
				}
			}

			output.Content.SetHtmlContent(content.ToString());
		}
	}
}

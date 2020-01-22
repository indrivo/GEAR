using System;
using System.Text;
using System.Threading.Tasks;
using GR.PageRender.Abstractions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GR.WebApplication.TagHelpers
{
	[HtmlTargetElement("application-styles")]
	public sealed class LayoutCssScriptsTagHelper : TagHelper
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

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "div";
			var content = new StringBuilder();
			if (LayoutId == null)
			{
				output.Content.SetHtmlContent(string.Empty);
				return;
			}

			var styles = await _pageRender.GetPageStyles(LayoutId.Value);
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
	public sealed class LayoutJsScriptsTagHelper : TagHelper
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

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "div";
			if (LayoutId == null)
			{
				output.Content.SetHtmlContent(string.Empty);
				return;
			}
			var scripts = await _pageRender.GetPageScripts(LayoutId.Value);
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

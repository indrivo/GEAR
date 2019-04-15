using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ST.CORE.Services.Abstraction;

namespace ST.CORE.TagHelpers
{
	[HtmlTargetElement("application")]
	public class ApplicationTagHelper : TagHelper
	{
		/// <summary>
		/// Inject page render
		/// </summary>
		private readonly IPageRender _pageRender;

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
			var initial = (await output.GetChildContentAsync()).GetContent();
			var (pre, next) = await _pageRender.GetLayoutHtml(LayoutId);
			var content = new StringBuilder();
			content.Append(pre);
			content.Append(initial);
			content.Append(next);
			output.Content.SetHtmlContent(content.ToString());
		}
	}
}

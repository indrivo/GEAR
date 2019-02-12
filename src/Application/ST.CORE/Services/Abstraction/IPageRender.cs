using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using ST.BaseBusinessRepository;
using ST.Entities.Models.Pages;

namespace ST.CORE.Services.Abstraction
{
	public interface IPageRender
	{
		string GetLayoutJavaScript(Guid? layoutId = null);
		(HtmlString, HtmlString) GetLayoutHtml(Guid? layoutId = null);
		string GetLayoutCss(Guid? layoutId = null);
		HtmlString GetPageHtml(string pageName);
		string GetPageCss(string pageName);
		string GetPageJavaScript(string pageName);
		ResultModel SavePageContent(Guid pageId, string html, string css, string js = null);
		ResultModel<string> CreatePage(string name);
		ResultModel DeletePage(string path);
		ResultModel UpdatePageName(string path, string oldName, string newName);
		ResultModel<IEnumerable<PageScript>> GetPageScripts(Guid pageId);
		ResultModel<IEnumerable<PageStyle>> GetPageStyles(Guid pageId);

	}
}

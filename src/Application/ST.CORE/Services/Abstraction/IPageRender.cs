using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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
		ResultModel<IEnumerable<PageScript>> GetPageScripts(Guid pageId);
		ResultModel<IEnumerable<PageStyle>> GetPageStyles(Guid pageId);
		Task<ResultModel<Guid>> GenerateListPageType([Required] string name, string path, [Required] Guid viewModelId);
		Task<ResultModel<Guid>> GenerateViewModel(Guid entityId);
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using ST.Core.Helpers;
using ST.Entities.Models.Pages;

namespace ST.Configuration.Services.Abstraction
{
    public interface IPageRender
    {
        Task<string> GetLayoutJavaScript(Guid? layoutId = null);
        Task<(HtmlString, HtmlString)> GetLayoutHtml(Guid? layoutId = null);
        Task<string> GetLayoutCss(Guid? layoutId = null);
        Task<HtmlString> GetPageHtml(string pageName);
        Task<string> GetPageCss(string pageName);
        Task<string> GetPageJavaScript(string pageName);
        Task<ResultModel> SavePageContent(Guid pageId, string html, string css, string js = null);
        Task<ResultModel<IEnumerable<PageScript>>> GetPageScripts(Guid pageId);
        Task<ResultModel<IEnumerable<PageStyle>>> GetPageStyles(Guid pageId);
        Task<ResultModel<Guid>> GenerateListPageType([Required] string name, string path,
            [Required] Guid viewModelId, string addPath = "#", string editPath = "#");
        Task<ResultModel<Guid>> GenerateViewModel(Guid entityId);
        Task<ResultModel> GenerateFormPage(Guid formId, string path, string pageName);
        Task<Page> GetPageAsync(Guid? pageId);
    }
}

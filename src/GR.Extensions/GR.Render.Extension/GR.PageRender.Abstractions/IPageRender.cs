using GR.Core.Helpers;
using GR.PageRender.Abstractions.Models.Pages;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers.Filters;

namespace GR.PageRender.Abstractions
{
    public interface IPageRender
    {
        /// <summary>
        /// Get javascript layout
        /// </summary>
        /// <param name="layoutId"></param>
        /// <returns></returns>
        Task<string> GetLayoutJavaScript(Guid? layoutId = null);
        /// <summary>
        /// Get html layout
        /// </summary>
        /// <param name="layoutId"></param>
        /// <returns></returns>
        Task<(HtmlString, HtmlString)> GetLayoutHtml(Guid? layoutId = null);
        /// <summary>
        /// Get css layout
        /// </summary>
        /// <param name="layoutId"></param>
        /// <returns></returns>
        Task<string> GetLayoutCss(Guid? layoutId = null);
        /// <summary>
        /// Get html by page name
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        Task<HtmlString> GetPageHtml(string pageName);
        /// <summary>
        /// Get css by page name
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        Task<string> GetPageCss(string pageName);

        /// <summary>
        /// Get js by page name
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        Task<string> GetPageJavaScript(string pageName);

        /// <summary>
        /// Save page content
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="html"></param>
        /// <param name="css"></param>
        /// <param name="js"></param>
        /// <returns></returns>
        Task<ResultModel> SavePageContent(Guid pageId, string html, string css, string js = null);

        /// <summary>
        /// Get page scripts
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<PageScript>>> GetPageScripts(Guid pageId);

        /// <summary>
        /// Get page styles
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<PageStyle>>> GetPageStyles(Guid pageId);

        /// <summary>
        /// Generate list page type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="viewModelId"></param>
        /// <param name="addPath"></param>
        /// <param name="editPath"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> GenerateListPageType([Required] string name, string path,
            [Required] Guid viewModelId, string addPath = "#", string editPath = "#");

        /// <summary>
        /// Generate view model for entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> GenerateViewModel(Guid entityId);

        /// <summary>
        /// Generate form page
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="path"></param>
        /// <param name="pageName"></param>
        /// <returns></returns>
        Task<ResultModel> GenerateFormPage(Guid formId, string path, string pageName);
        /// <summary>
        /// Get page by id
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        Task<Page> GetPageAsync(Guid? pageId);

        /// <summary>
        /// Filter jquery data table request
        /// </summary>
        /// <param name="param"></param>
        /// <param name="viewModelId"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<DTResult<object>> FilterJqueryDataTableRequestAsync(DTParameters param, Guid? viewModelId, ICollection<Filter> filters);
    }
}

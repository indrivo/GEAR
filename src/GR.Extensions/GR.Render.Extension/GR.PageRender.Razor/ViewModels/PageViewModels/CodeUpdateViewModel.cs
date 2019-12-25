using System;

namespace GR.PageRender.Razor.ViewModels.PageViewModels
{
    public class CodeUpdateViewModel
    {
        public string CssCode { get; set; }
        public string HtmlCode { get; set; }
        public Guid PageId { get; set; }
    }
}
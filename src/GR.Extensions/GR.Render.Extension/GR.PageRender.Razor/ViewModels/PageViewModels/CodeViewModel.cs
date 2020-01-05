using System;

namespace GR.PageRender.Razor.ViewModels.PageViewModels
{
    public class CodeViewModel
    {
        public Guid PageId { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace GR.PageRender.Razor.ViewModels.PageViewModels
{
    public class PostFormViewModel
    {
        public Dictionary<string, string> Data { get; set; }
        public Guid FormId { get; set; }
        public bool IsEdit { get; set; }
        public IEnumerable<SystemFieldViewModel> SystemFields { get; set; }
    }

    public class SystemFieldViewModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
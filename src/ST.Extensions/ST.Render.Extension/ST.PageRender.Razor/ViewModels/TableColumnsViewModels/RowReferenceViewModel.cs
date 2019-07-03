using System.Collections.Generic;

namespace ST.PageRender.Razor.ViewModels.TableColumnsViewModels
{
    public class RowReferenceViewModel
    {
        public IEnumerable<Dictionary<string, object>> Data { get; set; }
        public string EntityName { get; set; }
    }
}
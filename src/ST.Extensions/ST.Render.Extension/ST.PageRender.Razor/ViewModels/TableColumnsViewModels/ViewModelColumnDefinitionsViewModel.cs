using System.Collections.Generic;
using ST.PageRender.Abstractions.Models.ViewModels;

namespace ST.PageRender.Razor.ViewModels.TableColumnsViewModels
{
    public class ViewModelColumnDefinitionsViewModel
    {
        public IEnumerable<TableColumnData> EntityFields { get; set; }
        public IEnumerable<ViewModelFields> ViewModelFields { get; set; }
    }
}

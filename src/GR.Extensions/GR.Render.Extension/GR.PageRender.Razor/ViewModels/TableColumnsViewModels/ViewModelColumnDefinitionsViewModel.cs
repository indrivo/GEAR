using System.Collections.Generic;
using GR.PageRender.Abstractions.Models.ViewModels;

namespace GR.PageRender.Razor.ViewModels.TableColumnsViewModels
{
    public class ViewModelColumnDefinitionsViewModel
    {
        public IEnumerable<TableColumnData> EntityFields { get; set; }
        public IEnumerable<ViewModelFields> ViewModelFields { get; set; }
    }
}

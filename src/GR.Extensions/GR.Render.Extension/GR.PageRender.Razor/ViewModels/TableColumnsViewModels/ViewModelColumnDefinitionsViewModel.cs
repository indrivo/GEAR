using GR.PageRender.Abstractions.Models.ViewModels;
using System.Collections.Generic;

namespace GR.PageRender.Razor.ViewModels.TableColumnsViewModels
{
    public class ViewModelColumnDefinitionsViewModel
    {
        public IEnumerable<TableColumnData> EntityFields { get; set; }
        public IEnumerable<ViewModelFields> ViewModelFields { get; set; }
    }
}
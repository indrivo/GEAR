using GR.Entities.Abstractions.Models.Tables;
using System;

namespace GR.PageRender.Razor.ViewModels.TableColumnsViewModels
{
    public class TableColumnData : TableModelField
    {
        public Guid? ColumnId { get; set; }
    }
}
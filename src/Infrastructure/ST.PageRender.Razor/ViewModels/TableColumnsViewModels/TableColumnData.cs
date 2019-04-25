using System;
using ST.Entities.Models.Tables;

namespace ST.PageRender.Razor.ViewModels.TableColumnsViewModels
{
	public class TableColumnData : TableModelFields
	{
		public Guid? ColumnId { get; set; }
	}
}

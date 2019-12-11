using System;
using GR.Entities.Abstractions.Models.Tables;

namespace GR.PageRender.Razor.ViewModels.TableColumnsViewModels
{
	public class TableColumnData : TableModelField
	{
		public Guid? ColumnId { get; set; }
	}
}

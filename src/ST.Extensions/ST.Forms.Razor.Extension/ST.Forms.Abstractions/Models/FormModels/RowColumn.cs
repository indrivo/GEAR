using System;
using ST.Core;

namespace ST.Forms.Abstractions.Models.FormModels
{
    public class RowColumn : BaseModel
    {
        public Guid RowId { get; set; }
        public Guid ColumnId { get; set; }
    }
}
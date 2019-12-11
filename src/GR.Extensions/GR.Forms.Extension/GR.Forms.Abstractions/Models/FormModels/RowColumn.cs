using System;
using GR.Core;

namespace GR.Forms.Abstractions.Models.FormModels
{
    public class RowColumn : BaseModel
    {
        public Guid RowId { get; set; }
        public Guid ColumnId { get; set; }
    }
}
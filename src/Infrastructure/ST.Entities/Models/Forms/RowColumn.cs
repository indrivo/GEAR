using System;
using ST.Core;

namespace ST.Entities.Models.Forms
{
    public class RowColumn : ExtendedModel
    {
        public Guid RowId { get; set; }
        public Guid ColumnId { get; set; }
    }
}
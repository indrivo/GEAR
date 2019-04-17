using System;
using ST.Shared;

namespace ST.Entities.Models.Forms
{
    public class RowColumn : ExtendedModel
    {
        public Guid RowId { get; set; }
        public Guid ColumnId { get; set; }
    }
}
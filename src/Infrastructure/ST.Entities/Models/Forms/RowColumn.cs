using System;
using ST.Audit.Models;

namespace ST.Entities.Models.Forms
{
    public class RowColumn : ExtendedModel
    {
        public Guid RowId { get; set; }
        public Guid ColumnId { get; set; }
    }
}
using System;
using ST.Audit.Models;

namespace ST.Entities.Models.Forms
{
    public class ColumnField : ExtendedModel
    {
        public Guid ColumnId { get; set; }
        public Guid FieldId { get; set; }
        public int Order { get; set; }
    }
}
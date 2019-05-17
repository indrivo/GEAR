using System;
using ST.Core;

namespace ST.Forms.Abstractions.Models.FormModels
{
    public class ColumnField : BaseModel
    {
        public Guid ColumnId { get; set; }
        public Guid FieldId { get; set; }
        public int Order { get; set; }
    }
}
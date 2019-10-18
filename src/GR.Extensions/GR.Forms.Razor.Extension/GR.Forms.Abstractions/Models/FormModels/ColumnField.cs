using System;
using GR.Core;

namespace GR.Forms.Abstractions.Models.FormModels
{
    public class ColumnField : BaseModel
    {
        public Guid ColumnId { get; set; }
        public Guid FieldId { get; set; }
        public int Order { get; set; }
    }
}
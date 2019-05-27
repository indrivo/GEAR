using System;
using ST.Core;

namespace ST.Forms.Abstractions.Models.FormModels
{
    public class StageRows : BaseModel
    {
        public Guid StageId { get; set; }
        public Guid RowId { get; set; }
    }
}
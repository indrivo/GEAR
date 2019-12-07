using System;
using GR.Core;

namespace GR.Forms.Abstractions.Models.FormModels
{
    public class StageRows : BaseModel
    {
        public Guid StageId { get; set; }
        public Guid RowId { get; set; }
    }
}
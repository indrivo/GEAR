using System;
using ST.Core;

namespace ST.Entities.Models.Forms
{
    public class StageRows : BaseModel
    {
        public Guid StageId { get; set; }
        public Guid RowId { get; set; }
    }
}
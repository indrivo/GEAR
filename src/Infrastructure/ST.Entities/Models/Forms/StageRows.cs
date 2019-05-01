using System;
using ST.Core;

namespace ST.Entities.Models.Forms
{
    public class StageRows : ExtendedModel
    {
        public Guid StageId { get; set; }
        public Guid RowId { get; set; }
    }
}
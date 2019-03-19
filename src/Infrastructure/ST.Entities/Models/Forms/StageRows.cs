using System;
using ST.Audit.Models;

namespace ST.Entities.Models.Forms
{
    public class StageRows : ExtendedModel
    {
        public Guid StageId { get; set; }
        public Guid RowId { get; set; }
    }
}
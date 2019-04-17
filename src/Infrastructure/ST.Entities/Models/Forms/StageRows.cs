using System;
using ST.Shared;

namespace ST.Entities.Models.Forms
{
    public class StageRows : ExtendedModel
    {
        public Guid StageId { get; set; }
        public Guid RowId { get; set; }
    }
}
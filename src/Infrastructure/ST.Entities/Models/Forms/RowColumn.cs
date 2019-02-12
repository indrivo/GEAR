using System;
using ST.BaseRepository;

namespace ST.Entities.Models.Forms
{
    public class RowColumn : BaseModel
    {
        public Guid RowId { get; set; }
        public Guid ColumnId { get; set; }
    }
}
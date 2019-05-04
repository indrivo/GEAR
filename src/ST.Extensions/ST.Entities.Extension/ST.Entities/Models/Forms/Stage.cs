using System;
using System.Collections.Generic;
using ST.Core;

namespace ST.Entities.Models.Forms
{
    public class Stage : BaseModel
    {
        public Settings Settings { get; set; }
        public IEnumerable<Row> Rows { get; set; }
        public Guid FormId { get; set; }
    }
}
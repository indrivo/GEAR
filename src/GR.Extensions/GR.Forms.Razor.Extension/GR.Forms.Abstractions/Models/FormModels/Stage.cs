using System;
using System.Collections.Generic;
using GR.Core;

namespace GR.Forms.Abstractions.Models.FormModels
{
    public class Stage : BaseModel
    {
        public Settings Settings { get; set; }
        public IEnumerable<Row> Rows { get; set; }
        public Guid FormId { get; set; }
    }
}
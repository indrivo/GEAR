using System;
using System.Collections.Generic;
using GR.Core;

namespace GR.Forms.Abstractions.Models.FormModels
{
    public class Row : BaseModel
    {
        public IEnumerable<Column> Columns { get; set; }
        public Config Config { get; set; }
        public Guid ConfigId { get; set; }
        public List<Attrs> Attrs { get; set; }
        public Guid FormId { get; set; }
    }
}
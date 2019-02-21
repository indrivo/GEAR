using System;
using System.Collections.Generic;

namespace ST.Entities.Models.Forms
{
    public class Row : ExtendedModel
    {
        public IEnumerable<Column> Columns { get; set; }
        public Config Config { get; set; }
        public Guid ConfigId { get; set; }
        public Attrs Attrs { get; set; }
        public Guid AttrsId { get; set; }
        public Guid FormId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using ST.Shared;

namespace ST.Entities.Models.Forms
{
    public class Row : ExtendedModel
    {
        public IEnumerable<Column> Columns { get; set; }
        public Config Config { get; set; }
        public Guid ConfigId { get; set; }
        public List<Attrs> Attrs { get; set; }
        public Guid FormId { get; set; }
    }
}
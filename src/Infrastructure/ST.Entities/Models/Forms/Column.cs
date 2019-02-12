using System;
using System.Collections.Generic;
using ST.BaseRepository;

namespace ST.Entities.Models.Forms
{
    public class Column : BaseModel
    {
        public IEnumerable<Field> Fields { get; set; }
        public Config Config { get; set; }
        public Guid ConfigId { get; set; }
        public string ClassName { get; set; }
        public Guid FormId { get; set; }
    }
}
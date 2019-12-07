using System;
using System.Collections.Generic;
using GR.Core;

namespace GR.Forms.Abstractions.Models.FormModels
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
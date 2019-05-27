﻿using System;
using System.Collections.Generic;
using ST.Core;

namespace ST.Forms.Abstractions.Models.FormModels
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
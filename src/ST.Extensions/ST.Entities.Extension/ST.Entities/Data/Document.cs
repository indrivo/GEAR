﻿using ST.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ST.Entities.Data
{
   public class Document : BaseModel
    {
        public string CodDocument { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TargetGroup { get; set; }
        public string File { get; set; }
        public string Link { get; set; }
        public string Comment { get; set; }
        public string Extension { get; set; }
        public string Status { get; set; }
    }
}

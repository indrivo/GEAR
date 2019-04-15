﻿using System;
using System.Collections.Generic;
using ST.Audit.Models;

namespace ST.Entities.Models.Forms
{
    public class Stage : ExtendedModel
    {
        public Settings Settings { get; set; }
        public IEnumerable<Row> Rows { get; set; }
        public Guid FormId { get; set; }
    }
}
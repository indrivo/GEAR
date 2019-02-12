﻿using System;
using ST.BaseRepository;

namespace ST.Entities.Models.Forms
{
    public class ColumnField : BaseModel
    {
        public Guid ColumnId { get; set; }
        public Guid FieldId { get; set; }
        public int Order { get; set; }
    }
}
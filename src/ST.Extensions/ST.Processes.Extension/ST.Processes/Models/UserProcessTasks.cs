﻿using System;
using ST.Core;

namespace ST.Procesess.Models
{
    public class UserProcessTasks : BaseModel
    {
        public STProcessTask ProcessTask { get; set; }
        public Guid ProcessTaskId { get; set; }
        public Guid UserId { get; set; }
    }
}
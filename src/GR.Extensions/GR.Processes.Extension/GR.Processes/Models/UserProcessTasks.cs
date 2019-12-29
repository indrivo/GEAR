using System;
using GR.Core;

namespace GR.Procesess.Models
{
    public class UserProcessTasks : BaseModel
    {
        public STProcessTask ProcessTask { get; set; }
        public Guid ProcessTaskId { get; set; }
        public Guid UserId { get; set; }
    }
}

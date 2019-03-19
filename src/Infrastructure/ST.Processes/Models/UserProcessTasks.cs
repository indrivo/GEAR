using ST.BaseRepository;
using ST.Entities.Models;
using System;
using ST.Audit.Models;

namespace ST.Procesess.Models
{
    public class UserProcessTasks : ExtendedModel
    {
        public STProcessTask ProcessTask { get; set; }
        public Guid ProcessTaskId { get; set; }
        public Guid UserId { get; set; }
    }
}

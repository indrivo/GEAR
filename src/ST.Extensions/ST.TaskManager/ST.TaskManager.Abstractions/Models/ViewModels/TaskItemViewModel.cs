using System;
using System.Collections.Generic;
using System.Text;

namespace ST.TaskManager.Abstractions.Models.ViewModels
{
    public class TaskItemViewModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public bool IsDone { get; set; }
        public Guid TaskId { get; set; }
    }
}

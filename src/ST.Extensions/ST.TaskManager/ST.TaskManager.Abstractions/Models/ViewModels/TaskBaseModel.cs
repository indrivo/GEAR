using ST.TaskManager.Abstractions.Enums;
using System;

namespace ST.TaskManager.Abstractions.Models.ViewModels
{
    public class TaskBaseModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid UserId { get; set; }

        public TaskPriority TaskPriority { get; set; }

        public TaskStatus Status { get; set; }
    }
}

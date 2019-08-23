using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ST.TaskManager.Abstractions.Enums;

namespace ST.TaskManager.Abstractions.Models.ViewModels
{
    public class CreateTaskViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid UserId { get; set; }

        public TaskPriority TaskPriority { get; set; }

        public TaskStatus Status { get; set; }


        public virtual List<TaskItemViewModel> TaskItems { get; set; }
    }
}

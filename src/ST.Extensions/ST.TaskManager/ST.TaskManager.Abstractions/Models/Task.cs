using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Core;
using ST.Identity.Abstractions;
using ST.TaskManager.Abstractions.Enums;

namespace ST.TaskManager.Abstractions.Models
{
    public class Task : BaseModel
    {
        /// <summary>
        /// Task name
        /// </summary>
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid UserId { get; set; }

        public TaskPriority TaskPriority { get; set; }

        public TaskStatus Status { get; set; }

        public virtual ICollection<TaskItem> TaskItems { get; set; }
    }
}

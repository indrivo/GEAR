using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Core;
using ST.TaskManager.Abstractions.Enums;

namespace ST.TaskManager.Abstractions.Models
{
    public class Task : BaseModel
    {
        /// <summary>
        /// Task Name
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Task description
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Task start date
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Task end date
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// User Id for assignment
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Task Priority Status
        /// </summary>
        [Required]
        public TaskPriority TaskPriority { get; set; }


        /// <summary>
        /// Task Status
        /// </summary>
        [Required]
        public TaskStatus Status { get; set; }

        /// <summary>
        /// Task items(sub tasks)
        /// </summary>
        public virtual ICollection<TaskItem> TaskItems { get; set; }
    }
}

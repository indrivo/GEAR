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
        public virtual string Name { get; set; }

        /// <summary>
        /// Task description
        /// </summary>
        [Required]
        [MaxLength(500)]
        public virtual string Description { get; set; }

        /// <summary>
        /// Task start date
        /// </summary>
        [Required]
        public virtual DateTime StartDate { get; set; }

        /// <summary>
        /// Task end date
        /// </summary>
        [Required]
        public virtual DateTime EndDate { get; set; }

        /// <summary>
        /// User Id for assignment
        /// </summary>
        [Required]
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Task Priority Status
        /// </summary>
        [Required]
        public virtual TaskPriority TaskPriority { get; set; }


        /// <summary>
        /// Task Status
        /// </summary>
        [Required]
        public virtual TaskStatus Status { get; set; }

        /// <summary>
        /// Task items(sub tasks)
        /// </summary>
        public virtual ICollection<TaskItem> TaskItems { get; set; }
    }
}

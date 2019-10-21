using GR.TaskManager.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.TaskManager.Abstractions.Models.ViewModels
{
    public class TaskBaseModel
    {
        [Required]
        [MaxLength(50)]
        public virtual string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string Description { get; set; }

        [Required]
        public virtual DateTime StartDate { get; set; }

        [Required]
        public virtual DateTime EndDate { get; set; }

        [Required]
        public virtual Guid UserId { get; set; }

        public virtual List<Guid> Files { get; set; }

        /// <summary>
        /// Assigned users
        /// </summary>
        public virtual IEnumerable<Guid> UserTeam { get; set; } = new List<Guid>();

        [Required]
        public virtual TaskPriority TaskPriority { get; set; }

        [Required]
        public virtual TaskStatus Status { get; set; }
    }
}

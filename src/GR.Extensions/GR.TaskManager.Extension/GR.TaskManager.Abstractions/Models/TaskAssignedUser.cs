using System;
using System.ComponentModel.DataAnnotations;

namespace GR.TaskManager.Abstractions.Models
{
    public class TaskAssignedUser
    {
        /// <summary>
        /// User id
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Task reference
        /// </summary>
        [Required]
        public virtual Guid TaskId { get; set; }
        public virtual Task Task { get; set; }
    }
}

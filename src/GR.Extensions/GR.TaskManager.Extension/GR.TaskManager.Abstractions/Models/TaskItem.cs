using System;
using System.ComponentModel.DataAnnotations;

namespace GR.TaskManager.Abstractions.Models
{
    public class TaskItem
    {
        public TaskItem()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Stores Id of the Object
        /// </summary>
        public Guid Id { get; set; }


        /// <summary>
        /// Task item name
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }


        /// <summary>
        /// Task item status
        /// </summary>
        public bool IsDone { get; set; }

        /// <summary>
        /// Reference to parent task
        /// </summary>
        public virtual Task Task { get; set; }
    }
}

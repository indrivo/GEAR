using System;
using System.ComponentModel.DataAnnotations;

namespace ST.TaskManager.Abstractions.Models
{
    public class TaskItem
    {
        public TaskItem()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>Stores Id of the Object</summary>
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsDone { get; set; }
        public virtual Task Task { get; set; }
    }
}

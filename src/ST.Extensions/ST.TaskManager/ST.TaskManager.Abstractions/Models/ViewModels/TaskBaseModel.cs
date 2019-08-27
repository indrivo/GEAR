using ST.TaskManager.Abstractions.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ST.TaskManager.Abstractions.Models.ViewModels
{
    public class TaskBaseModel
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public TaskPriority TaskPriority { get; set; }

        [Required]
        public TaskStatus Status { get; set; }
    }
}

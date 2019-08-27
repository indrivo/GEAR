using System;
using System.ComponentModel.DataAnnotations;


namespace ST.TaskManager.Abstractions.Models.ViewModels
{
    public sealed class TaskItemViewModel
    {
        public Guid? Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public bool IsDone { get; set; }

        [Required]
        public Guid TaskId { get; set; }
    }
}

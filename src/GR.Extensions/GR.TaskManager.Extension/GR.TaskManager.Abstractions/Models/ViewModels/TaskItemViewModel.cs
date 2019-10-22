using System;
using System.ComponentModel.DataAnnotations;


namespace GR.TaskManager.Abstractions.Models.ViewModels
{
    public class TaskItemViewModel
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public bool IsDone { get; set; }
    }

    public class CreateTaskItemViewModel : TaskItemViewModel
    {
        [Required]
        public Guid TaskId { get; set; }
    }

    public class UpdateTaskItemViewModel : TaskItemViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public sealed class GetTaskItemViewModel : UpdateTaskItemViewModel
    {

    }

}

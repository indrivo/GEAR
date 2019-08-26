using System;

namespace ST.TaskManager.Abstractions.Models.ViewModels
{
    public class GetTaskViewModel : TaskBaseModel
    {
        public Guid Id { get; set; }
    }
}

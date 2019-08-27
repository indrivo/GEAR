using System;

namespace ST.TaskManager.Abstractions.Models.ViewModels
{
    public sealed class GetTaskViewModel : TaskBaseModel
    {
        public Guid Id { get; set; }
    }
}

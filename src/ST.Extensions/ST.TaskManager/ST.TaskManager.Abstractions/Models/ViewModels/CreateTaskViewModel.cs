using System.Collections.Generic;


namespace ST.TaskManager.Abstractions.Models.ViewModels
{
    public sealed class CreateTaskViewModel : TaskBaseModel
    {
        public List<TaskItemViewModel> TaskItems { get; set; }
    }
}

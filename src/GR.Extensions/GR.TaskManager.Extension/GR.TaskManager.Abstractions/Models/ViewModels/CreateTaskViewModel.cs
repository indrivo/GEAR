using System.Collections.Generic;

namespace GR.TaskManager.Abstractions.Models.ViewModels
{
    public sealed class CreateTaskViewModel : TaskBaseModel
    {
        /// <summary>
        /// Task items
        /// </summary>
        public List<TaskItemViewModel> TaskItems { get; set; }
    }
}

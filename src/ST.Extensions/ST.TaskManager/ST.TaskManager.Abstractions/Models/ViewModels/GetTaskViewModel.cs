using System;

namespace ST.TaskManager.Abstractions.Models.ViewModels
{
    public sealed class GetTaskViewModel : TaskBaseModel
    {
        /// <summary>
        /// Record id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Task number
        /// </summary>
        public string TaskNumber { get; set; }
    }
}

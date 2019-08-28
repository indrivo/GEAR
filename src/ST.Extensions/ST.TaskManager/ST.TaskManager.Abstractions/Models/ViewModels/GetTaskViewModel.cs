using System;

namespace ST.TaskManager.Abstractions.Models.ViewModels
{
    public sealed class GetTaskViewModel : TaskBaseModel
    {
        /// <summary>
        /// Record id
        /// </summary>
        public Guid Id { get; set; }
    }
}

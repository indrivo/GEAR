using System;
using GR.TaskManager.Abstractions.Enums;

namespace GR.TaskManager.Abstractions.Models.ViewModels
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

        /// <summary>
        /// Task items counted [completed/total]
        /// </summary>
        public int[] TaskItemsCount { get; set; }

        /// <summary>
        /// Author
        /// </summary>
        public string Author { get; set; } = "Undefined";

        /// <summary>
        /// Modified by
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Access level
        /// </summary>
        public string AccessLevel { get; set; } = TaskAccess.Undefined.ToString();
    }
}

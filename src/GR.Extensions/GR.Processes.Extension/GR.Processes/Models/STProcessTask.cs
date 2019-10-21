using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core;

namespace GR.Procesess.Models
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class STProcessTask : BaseModel
    {
        /// <summary>
        /// Name of task
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Description of task
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Process task state
        /// </summary>
        [Required]
        [DefaultValue(ProcessTaskState.Inactive)]
        public ProcessTaskState ProcessTaskState { get; set; }

        /// <summary>
        /// Life of task
        /// </summary>
        [Required]
        public int Life { get; set; }

        /// <summary>
        /// Assigned user who need to do this task
        /// </summary>
        public Guid? Assigned { get; set; }

        /// <summary>
        /// Process transition
        /// </summary>
        public STProcessTransition ProcessTransition { get; set; }
        public Guid ProcessTransitionId { get; set; }
    }

    public enum ProcessTaskState
    {
        Active, Completed, Inactive, InProgress
    }
}

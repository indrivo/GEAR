using ST.Audit.Attributes;
using ST.Audit.Enums;
using ST.Entities.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ST.Audit.Models;

namespace ST.Procesess.Models
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class STProcessTask : ExtendedModel
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
